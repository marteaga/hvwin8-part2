(function () {
    "use strict";

    var self;
    WinJS.Namespace.define("Application", {
        LoginManager: WinJS.Class.define(
            function LoginManager() {
                // save a ref
                self = this;

                // store this object globally
                Application.loginManager = this;
            },
            {
                _baseUrl: 'http://localhost:10190/api/v1/DoctorAccount/',

                // method to call when login is complete
                onLoginComplete: null,

                // login failed
                onLoginFailed: null,

                // method to call when signout occurs
                onSignOut: null,

                // method to call if signout fails for some reason
                onSignOutFailed: null,

                // check access and let the caller know the success
                ping: function (success, fail) {
                    WinJS.xhr({
                        type: 'POST',
                        headers: { "Content-type": "application/x-www-form-urlencoded" },
                        url: this._baseUrl + 'Ping'
                    }).then(
                        function (result) {
                            try {
                                var res = JSON.parse(result.responseText);
                                if (res.status === 'ok') {
                                    // we are ok to make request
                                    if (success)
                                        success();
                                }
                                else {
                                    // we are not ok 
                                    if (fail)
                                        fail(result);
                                }
                            }
                            catch (e) {
                                // just assume there is no cookie saved
                                if (fail)
                                    fail()
                            }

                        },
                        function (result) {
                            // there was an error so let the caller know
                            if (fail)
                                fail(result);
                        });
                },

                // login method
                login: function (username, password) {
                    // make a request
                    WinJS.xhr({
                        type: 'POST',
                        url: this._baseUrl + 'Login',
                        headers: { "Content-type": "application/x-www-form-urlencoded" },
                        data: 'name=' + username + '&password=' + password
                    }).then(
                    function (result) {
                        if (result.status === 200) {
                            // we are good see if the status code is ok
                            var res = JSON.parse(result.responseText);
                            if (res.status === 'ok') {
                                // we are ok so callback
                                if (self.onLoginComplete)
                                    self.onLoginComplete(result);
                            }
                            else {
                                // tell the user we can't login
                                if (self.onLoginFailed)
                                    self.onLoginFailed(result);
                            }
                        }
                        else {
                            // there was a wrong reposne from server
                            if (self.onLoginFailed)
                                self.onLoginFailed(result);
                        }
                    },
                    function (result) {
                        // there was an error so report back to user
                        if (self.onLoginFailed)
                            self.onLoginFailed(result);
                    });
                },

                // signs out the current session
                signOut: function () {
                    // make a request
                    WinJS.xhr({
                        url: this._baseUrl + 'SignOut',
                    }).then(
                    function (result) {
                        if (result.status === 200) {
                            // we are good see if the status code is ok
                            var res = JSON.parse(result.responseText);
                            if (res.status === 'ok') {
                                // we are ok so callback
                                if (self.onSignOut)
                                    self.onSignOut(result);
                            }
                            else {
                                // tell the user we can't login
                                if (self.onSignOutFailed)
                                    self.onSignOutFailed(result);
                            }
                        }
                        else {
                            // there was a wrong reposne from server
                            if (self.onSignOutFailed)
                                self.onSignOutFailed(result);
                        }
                    },
                    function (result) {
                        // there was an error so report back to user
                        if (self.onSignOutFailed)
                            self.onSignOutFailed(result);
                    });
                }
            }
        )
    });
})();