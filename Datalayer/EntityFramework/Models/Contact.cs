using System;
using System.Collections.Generic;
using System.Text;

namespace DataLayer.EntityFramework.Models;
public class Contact: BaseEntity
{
	public string full_name { get; set; }
	public string mobile { get; set; }
	public string message { get; set; }
}
