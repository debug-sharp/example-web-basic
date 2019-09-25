using Desharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace WebApp {
    // https://docs.microsoft.com/cs-cz/dotnet/api/system.web.httpapplication?view=netframework-4.8
	public partial class Global : System.Web.HttpApplication, IHttpHandler, IRequiresSessionState {
        




        /**
         * Global.aspx HttpApplication handlers:
         */

        // Called only once per session:
        protected void Session_Start(object o, EventArgs e) {
            HttpContext.Current.Session.Add("RequestCounter", 0);
        }
         
        // Change rewritten url back to raw url at the request begin:
        protected void Application_BeginRequest (object o, EventArgs e) {
            this.Request
                .GetType()
                    .GetField("_url", BindingFlags.Instance | BindingFlags.NonPublic)
                        .SetValue(this.Request, new System.Uri(
                            this.Request.Url.Scheme + System.Uri.SchemeDelimiter +
                            this.Request.Url.Authority + this.Request.RawUrl
                        ));
		}
        // Read session data:
		protected void Application_PostAuthorizeRequest (object o, EventArgs e) {
            HttpContext.Current.SetSessionStateBehavior(SessionStateBehavior.Required);
        }
        // Handle all requests execution:
		protected void Application_PreRequestHandlerExecute (object o, EventArgs e) {
			if (this.Request.Url.LocalPath.IndexOf("/test-redirect") == 0) {
				this._demoHandleTestRecorect();
			} else {
				this._demoHandleAllRequests();
			}
		}
		// Add execution time header after everything is done:
		protected void Application_PostRequestHandlerExecute (object o, EventArgs e) {
			TimeSpan reqTimeSpan = DateTime.Now - HttpContext.Current.Timestamp;
			string requestTime = reqTimeSpan.Milliseconds.ToString("0.###", new CultureInfo("en-US")) + " ms (" 
					+ reqTimeSpan.Ticks.ToString() + " ticks)";
			this.Response.AddHeader("X-Exec-Time", requestTime);
		}






        /**
         * Demo handler methods: 
         */

        // Handle demo redirect request:
		private void _demoHandleTestRecorect () {
			Debug.Dump("Redirected from: " + this.Request.Url.AbsoluteUri);
			this.Response.Headers.Add("Location", "/");
		    this.Response.StatusCode = 302;
		}
        // Handle all demo requests:
		private void _demoHandleAllRequests () {
			// Write some standard output:
			this.Response.Write("Hallo world!<br /><br />");

			// Try to dump something in debug mode:
            try {
				this._demoDumpAndLog();
				this._demoSession();
				//this._demoCatchedException();
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
			this.Response.Write(@"<br /><br />Click for <a href=""/test-redirect"">demo redirection</a>.");
		}





        /**
         * Demo dump methods: 
         */
        
        // Dump some structured example data:
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

        // Count something in session:
        private void _demoSession() {
            int requestsCount = (int)this.Session["RequestCounter"];
            this.Session["RequestCounter"] = requestsCount + 1;
        }
        
        // Render some catched exception:
        private void _demoCatchedException () {
            try {
                throw new Exception("Demo catched exception text.");
            } catch (Exception ex) {
				Debug.Dump(ex); 
                Debug.Log(ex);
            }
        }
        
        // Render some uncatched exception:
        private void _demoUncatchedException () {
			throw new Exception("Demo uncatched exception text.");
        }
        
        // Run test dumps:
        private  void _runDumpingTests () {
		    var dlTest = new Tests.DumpingAndLoging();
		    dlTest.TestAll();
        }

        // Run test exceptions rendering:
        private  void _runExceptionsTests () {
		    var eTest = new Tests.ExceptionsRendering();
		    eTest.TestAll();
        }
    }
}