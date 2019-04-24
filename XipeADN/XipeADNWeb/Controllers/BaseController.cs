using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using XipeADNWeb.Models;
using XipeADNWeb.Entities;


public abstract class BaseController : Controller
{
    User CurrentUser { get; }
}