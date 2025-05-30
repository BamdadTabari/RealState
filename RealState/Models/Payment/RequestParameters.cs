﻿namespace RealState;

public class RequestParameters
{
    public string merchant_id { get; set; }
    public long amount { get; set; }
    public string description { get; set; }
    public string callback_url { get; set; }

    public string[] metadata { get; set; }

    public RequestParameters(string merchant_id, long amount, string description, string callback_url, string mobile, string email)
    {
        this.merchant_id = merchant_id;
        this.amount = amount;
        this.description = description;
        this.callback_url = callback_url;
        this.metadata = new string[2];
        if (mobile != null)
        {
            this.metadata[0] = mobile;
        }
        if (email != null)
        {
            this.metadata[1] = email;
        }
    }
}
