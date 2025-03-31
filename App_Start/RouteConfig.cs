using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace YorkScjool
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // ✅ Landing Page should load first
            routes.MapRoute(
                name: "LandingPage",
                url: "", // Root URL
                defaults: new { controller = "Home", action = "Landing" }
            );

            // ✅ Route for Login (Explicit before Default)
            routes.MapRoute(
                name: "Login",
                url: "Account/Login",
                defaults: new { controller = "Account", action = "Login" }
            );

            // ✅ Route for Register
            routes.MapRoute(
                name: "Register",
                url: "Account/Register",
                defaults: new { controller = "Account", action = "Register" }
            );

            // ✅ Route for Student Dashboard
            routes.MapRoute(
                name: "StudentDashboard",
                url: "StudentDashboard",
                defaults: new { controller = "Student", action = "StudentDashboard" }
            );

            routes.MapRoute(
                name: "Admin",
                url: "Admin/{action}/{id}",
                defaults: new { controller = "Admin", action = "Index", id = UrlParameter.Optional }
            );


            // ✅ Default Route (Placed **after** specific routes)
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
