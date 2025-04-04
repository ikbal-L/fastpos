﻿using System.Net.Http;
using ServiceInterface.Model;

namespace ServiceInterface.Interface
{
    public interface IAuthentification
    {
        public string BaseUrl { get; set; }
        int Authenticate(string user, string password, Annex annex, Terminal terminal);
        HttpResponseMessage Authenticate(string user, string password, Terminal terminal, Annex annex);
        int Authenticate(User user);
    }
}
