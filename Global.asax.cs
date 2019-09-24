using Desharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace WebApp {
	public partial class Global : System.Web.HttpApplication, IHttpHandler, IRequiresSessionState {
		// https://docs.microsoft.com/cs-cz/dotnet/api/system.web.httpapplication?view=netframework-4.8
		protected void Application_PostAuthorizeRequest() {
            // to start session:
            HttpContext.Current.SetSessionStateBehavior(SessionStateBehavior.Required);
        }
        public void Session_Start(object sender, EventArgs e) {
            // Called once when session is started in first request:
            HttpContext.Current.Session.Add("RequestCounter", 0);
        }
		protected void Application_AcquireRequestState (object sender, EventArgs e) {
			// Called for every request:
			if (this.Request.Path == "/") {
				this._demoHandleHome();
			} else {
				this._demoRedirectAnyOtherPageToHome();
			}
		}
		protected void Application_PostAcquireRequestState (object sender, EventArgs e) {
			// Called for every request:
			if (this.Response.StatusCode >= 300 && this.Response.StatusCode <= 400) {
				// When request is redirected - call flush and end the request:
				this.Response.Flush();
				this.Response.End();
			} else {
				// Call `Reponse.Flush();` method (not `Response.End();`) to be able to see session panel
				TimeSpan reqTimeSpan = DateTime.Now - HttpContext.Current.Timestamp;
				string requestTime = reqTimeSpan.Milliseconds.ToString("0.###", new CultureInfo("en-US")) + " ms (" 
					 + reqTimeSpan.Ticks.ToString() + " ticks)";
				this.Response.AddHeader("X-Exec-Time", requestTime);
				this.Response.Flush();
			}
		}
		private void _demoRedirectAnyOtherPageToHome () {
			Debug.Dump("Redirected from: " + this.Request.Path);
			this.Response.Headers.Add("Location", "/");
			this.Response.StatusCode = 302;
		}
		private void _demoHandleHome () {
			// Write some standard output:
			this.Response.Write("Hallo ");

			// Try to dump something in debug mode:
            try {
				this._demoDumpAndLog();
				this._demoSession();
				this._demoCatchedException();
				//this._demoUncatchedException();
            } catch (Exception ex) {
				// Last exception is:
				// - always displayed over screen in debug mode:
				// - always logged automatically in non-debug mode:
				throw new Exception("Global request error.", ex);
            }

			// Run some test dumps:
			//this._runDumpingTests();
			//this._runExceptionsTests();

			// Write some standard output:
			this.Response.Write("world!");
		}
        private void _demoDumpAndLog() {
			if (!Debug.Enabled()) return;
            var demoObject = new Dictionary<string, object>() {
                { "clark", new {
                    name = "Clark",
                    surname = "Kent",
                    tshirtIdol = "chuck"
                } },
                { "chuck", new {
                    name = "Chuck",
                    surname = "Noris",
                    tshirtIdol = "bud"
                } },
                { "bud", new {
                    name = "Bud",
                    surname = "Spencer",
                    tshirtIdol = ""
                } }
            };

            string dumpedObject = Debug.Dump(demoObject, new DumpOptions {
                Return = true,
                SourceLocation = true
            });
            this.Response.Write(dumpedObject);
            
            Debug.Dump(demoObject);
            Debug.Log(demoObject);
        }
        private void _demoSession() {
            int requestsCount = (int)this.Session["RequestCounter"];
            this.Session["RequestCounter"] = requestsCount + 1;
        }
        private void _demoCatchedException () {
            try {
                throw new Exception("Demo catched exception text.");
            } catch (Exception ex) {
				Debug.Dump(ex); 
                Debug.Log(ex);
            }
        }
        private void _demoUncatchedException () {
			throw new Exception("Demo uncatched exception text.");
        }
        private  void _runDumpingTests () {
		    var dlTest = new Tests.DumpingAndLoging();
		    dlTest.TestAll();
        }
        private  void _runExceptionsTests () {
		    var eTest = new Tests.ExceptionsRendering();
		    eTest.TestAll();
        }
    }
}