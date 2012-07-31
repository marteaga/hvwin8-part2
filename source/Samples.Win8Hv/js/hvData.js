(function () {
    "use strict";

    var self;
    WinJS.Namespace.define("Application", {
        HealthVaultData: WinJS.Class.define(
            function HealthVaultData() {
                // save a ref
                self = this;

                // store this object globally
                Application.healthVaultData = this;
            },
            {
                _baseUrl: 'http://localhost:10190/api/v1/DoctorAccount/',

                // Gets a list of users in the system
                getUsers: function (success, fail) {
                    WinJS.xhr({
                        url: this._baseUrl + 'getuserlist'
                    }).then(
                        function (result) {
                            try {
                                var res = JSON.parse(result.responseText);
                                if (res.status === 'ok') {
                                    // we are ok 
                                    if (success)
                                        success(res.users);
                                }
                                else {
                                    // we are not user id must not exist
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

                // Gets a list of a users entries
                getUserEntries: function (userId, success, fail) {
                    WinJS.xhr({
                        url: this._baseUrl + 'getuserdata?userId=' + userId
                    }).then(
                        function (result) {
                            try {
                                var res = JSON.parse(result.responseText);
                                if (res.status === 'ok') {
                                    // we are ok
                                    if (success)
                                        success(res.data);
                                }
                                else {
                                    // we are not user id must not exist
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
            }
        )
    });
})();