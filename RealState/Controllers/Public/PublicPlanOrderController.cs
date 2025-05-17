using DataLayer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RealState.Models;
using RealState.Models.Payment;
using RestSharp;
using System.Text;

namespace RealState.Controllers.Public;
[Route("api/public-plan-order")]
[ApiController]
public class PublicPlanOrderController(IUnitOfWork unitOfWork, JwtTokenService tokenService) : ControllerBase
{
	private readonly IUnitOfWork _unitOfWork = unitOfWork;
	private readonly JwtTokenService _tokenService = tokenService;
	readonly string merchant = "05eec37e-6da6-44b1-9cf4-99c35b7b8b10";
	long amount;
	string authority;
	string description = "خرید";

	[HttpPost]
	[Route("current-user-id")]
	[ApiExplorerSettings(IgnoreApi = true)]
	public async Task<long> GetCurrentUserId()
	{
		var userId = _tokenService.GetUserIdFromClaims(User) ?? "0";
		var user = await _unitOfWork.UserRepository.GetUser(int.Parse(userId));
		return user.id;
	}

	[HttpPost]
	[Route("current-user")]
	[ApiExplorerSettings(IgnoreApi = true)]
	public async Task<User> GetCurrentUser()
	{
		var userId = _tokenService.GetUserIdFromClaims(User) ?? "0";
		var user = await _unitOfWork.UserRepository.GetUser(int.Parse(userId));
		return user;
	}

	[HttpGet]
	[Route("plans")]
	public async Task<IActionResult> Plans()
	{
		var plans = await _unitOfWork.PlanRepository.GetAll();
		if (plans.Count <= 0)
			return NotFound(new ResponseDto<PlanDto>()
			{
				data = null,
				message = "مقدار پلن در دیتابیس وجود ندارد",
				is_success = false,
				response_code = 404
			});
		return Ok(new ResponseDto<List<PlanDto>>()
		{
			data = plans.Select(x=> new PlanDto()
			{
				id = x.id,
				created_at = x.created_at,
				updated_at = x.updated_at,
				slug = x.slug,
				name = x.name,
				description = x.description,
				plan_months = x.plan_months,
				price = x.price,
				property_count = x.property_count,
			}).ToList(),
			message = "",
			is_success = true,
			response_code = 200
		});
		}

	[HttpPost]
	[Route("payment")]
	public async Task<IActionResult> SendReq([FromForm] long plan_id)
	{
		try
		{
			var plan = await _unitOfWork.PlanRepository.Get(plan_id);
			if (plan == null)
				return NotFound(new ResponseDto<PlanDto>()
				{
					data = null,
					is_success=false,
					message = "پلن با این ایدی پیدا نشد",
					response_code = 404
				});

			var user = await GetCurrentUser();
			amount = (long)plan.price * 10; // تبدیل تومان به ریال
			var userRequest = HttpContext.Request;
			var callbackUrl = $"{userRequest.Scheme}://{userRequest.Host}/Order/VerifyByHttpClient";
			RequestParameters Parameters = new(merchant, amount, description, callbackUrl, "", "");

			var client = new RestClient(URLs.requestUrl);
			Method method = Method.Post;

			var request = new RestRequest("", method);
			request.AddHeader("accept", "application/json");
			request.AddHeader("content-type", "application/json");
			request.AddJsonBody(Parameters);

			var requestresponse = client.ExecuteAsync(request).Result;

			JObject jo = JObject.Parse(requestresponse.Content);
			string errorscode = jo["errors"].ToString();
			JObject jodata = JObject.Parse(requestresponse.Content);
			string dataauth = jodata["data"].ToString();

			if (dataauth != "[]")
			{
				authority = jodata["data"]["authority"].ToString();
				string gatewayUrl = URLs.gateWayUrl + authority;
				var entity = new Order()
				{
					amount = amount,
					user_id = await GetCurrentUserId(),
					created_at = DateTime.Now,
					authority = authority,
					updated_at = DateTime.Now,
					slug = SlugHelper.GenerateSlug(amount.ToString() + authority + StampGenerator.CreateSecurityStamp(10)),
					mobile = user.mobile,
					email = user.email,
					username = user.user_name,
					plan_id = plan.id,
				};
				await _unitOfWork.OrderRepository.AddAsync(entity);
				await _unitOfWork.CommitAsync();

				return Ok(new ResponseDto<OrderResponse>()
				{
					data = new OrderResponse()
					{
						redirect_url = gatewayUrl,
						authority = authority
					},
					is_success = true,
					message = "در حال انتقال به صفحه ی بانک",
					response_code = 200
				});
			}
			else
			{
				return BadRequest(new ResponseDto<PaymentResponse>()
				{
					data = new PaymentResponse()
					{
						message = "تراکنش ناموفق",
						success = false,
						status_code = 500,
						ref_id = "خطای بانک"
					},
					is_success = false,
					message = "خطای بانک",
					response_code =500
				});
			}
		}
		catch
		{
			return BadRequest(new ResponseDto<PaymentResponse>()
			{
				data = new PaymentResponse()
				{
					message = "تراکنش ناموفق",
					success = false,
					status_code = 500,
					ref_id = "خطای سرور در فرستادن درخواست به بانک"
				},
				is_success = false,
				message = "خطای سرور در فرستادن درخواست به بانک",
				response_code = 500
			});
		}
	}

	[HttpPost]
	[Route("verify-payment")]
	public async Task<IActionResult> VerifyByHttpClient()
	{
		VerifyParameters parameters = new VerifyParameters();

		var authority = HttpContext.Request.Query["Authority"].ToString();

		Order ord = await _unitOfWork.OrderRepository.GetByAuthority(authority);
		if (ord == null)
		{
			return NotFound(new ResponseDto<PaymentResponse>()
			{
				data = new PaymentResponse()
				{
					message = "سفارش یافت نشد",
					success = false,
					status_code = -11,
					ref_id = "سفارش یافت نشد"
				},
				is_success = false,
				response_code = 404,
				message = "سفارش یافت نشد"
			});
		}

		parameters.authority = authority;
		parameters.amount = (int)ord.amount * 10; // تبدیل تومان به ریال
		parameters.merchant_id = merchant;

		using (HttpClient client = new HttpClient())
		{
			var json = JsonConvert.SerializeObject(parameters);
			HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

			HttpResponseMessage response = await client.PostAsync(URLs.verifyUrl, content);
			string responseBody = await response.Content.ReadAsStringAsync();

			JObject jodata = JObject.Parse(responseBody);
			string data = jodata["data"].ToString();
			string errors = jodata["errors"].ToString();

			if (data != "{}")
			{
				string refid = jodata["data"]["ref_id"].ToString();

				//ViewBag.code = refid;

				// اطلاعات پرداخت را ذخیره کنید
				ord.status = 100;
				ord.date_paid = DateTime.Now;
				ord.ref_id = refid;
				ord.card_number = jodata["data"]["card_pan"].ToString(); // شماره کارت
				ord.response_message = response.Content.ToString();
				_unitOfWork.OrderRepository.Update(ord);
				await _unitOfWork.CommitAsync();
				var plan = await _unitOfWork.PlanRepository.Get(ord.plan_id);
				if (plan == null)
					return NotFound(new ResponseDto<PlanDto>()
					{
						data = null,
						message = "پلن پیدا نشد",
						is_success = false,
						response_code = 404
					});
				var user = await GetCurrentUser();
				user.expire_date = DateTime.Now.AddMonths(plan.plan_months);
				user.property_count = plan.property_count;
				_unitOfWork.UserRepository.Update(user);
				await _unitOfWork.CommitAsync();

				return Ok(new ResponseDto<PaymentResponse>()
				{
					data = new PaymentResponse()
					{
						message = "تراکنش موفق",
						success = true,
						status_code = 100,
						ref_id = refid
					},
					is_success = true,
					message = "تراکنش موفق",
					response_code = 200,
				});
			}
			else
			{
				string refid = jodata["data"]["ref_id"].ToString();
				// اطلاعات پرداخت را ذخیره کنید
				ord.status = (int)response.StatusCode;
				ord.date_paid = DateTime.Now;
				ord.ref_id = "پرداخت انجام نشده است";
				ord.card_number = "پرداخت انجام نشده است"; // شماره کارت
				ord.response_message = response.Content.ToString();
				_unitOfWork.OrderRepository.Update(ord);
				await _unitOfWork.CommitAsync();
				return BadRequest(new ResponseDto<PaymentResponse>()
				{
					data = new PaymentResponse()
					{
						message = "تراکنش ناموفق",
						success = false,
						status_code = (int)response.StatusCode,
						ref_id = refid
					},
					is_success = true,
					message = "تراکنش ناموفق در وریفای تراکنش",
					response_code = 200,
				});
			}
		}
	}
}
