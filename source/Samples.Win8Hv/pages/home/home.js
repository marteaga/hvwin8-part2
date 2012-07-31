(function () {
    "use strict";

    var hvData;
    WinJS.UI.Pages.define("/pages/home/home.html", {
        // This function is called whenever a user navigates to this page. It
        // populates the page elements with the app's data.
        ready: function (element, options) {
            var self = this;
            hvData = new Application.HealthVaultData();

            // handle the on sign out method
            Application.loginManager.onSignOut = function () {
                self.status.innerText = "need to sign in";
                self.btnSignOut.innerText = "Sign In";
                userList.winControl.itemDataSource = new WinJS.Binding.List([]).dataSource;
            }

            // handle not being able to sign out
            Application.loginManager.onSignOutFailed = function () {
                // show a message to user and ask if they want to try again
                var msg = new Windows.UI.Popups.MessageDialog("Unable to sign out.");
                msg.commands.append(new Windows.UI.Popups.UICommand("Try again", function (command) {
                    Application.loginManager.signOut();
                }));
                msg.commands.append(new Windows.UI.Popups.UICommand("Close"));
                msg.defaultCommandIndex = 0;
                msg.cancelCommandIndex = 1;
                msg.showAsync();
            }

            // wire up the completed events
            Application.loginManager.onLoginComplete = function (result) {
                // we are good so attempt to get data
                console.log('sign in successful');
                self.status.innerText = "sign in successful";
                self.btnSignOut.innerText = "Sign Out";
                self.bindUsers();
            };

            // wire up the failed event
            Application.loginManager.onLoginFailed = function (result) {
                // show a message to user and ask if they want to try again
                var msg = new Windows.UI.Popups.MessageDialog("Unable to sign in.");
                msg.commands.append(new Windows.UI.Popups.UICommand("Try again", function (command) {
                    self.showSettings();
                }));
                msg.commands.append(new Windows.UI.Popups.UICommand("Close"));
                msg.defaultCommandIndex = 0;
                msg.cancelCommandIndex = 1;
                msg.showAsync();
            };

            // attempt to make the request
            Application.loginManager.ping(function () {
                // we are good
                self.status.innerText = "already logged in and cookie set";
                self.btnSignOut.innerText = "Sign Out";
                self.bindUsers();
            },
            function (result) {
                self.status.innerText = "need to sign in";
                self.btnSignOut.innerText = "Sign In";
                self.showSettings();
            });

            // wire up the sign out button
            self.btnSignOut.addEventListener('click', function () {
                if (this.innerText === "Sign In")
                    self.showSettings();
                else
                    Application.loginManager.signOut();
            });

            // wire up the item invoked event
            userList.addEventListener('iteminvoked', function (e) {
                var elem = document.getElementById("contenthost");

                // get the item that is selected
                var item = userList.winControl.itemDataSource.list.getItem(e.detail.itemIndex).data;

                // set the item globally so we can grab from entries page
                Application.selectedUser = item;

                // exit the content and when done navigate and enter the content
                WinJS.UI.Animation.exitContent(elem, null).then(function () {
                    // now navigate
                    WinJS.Navigation.navigate("/pages/userEntries/entries.html").then(function () {
                        WinJS.UI.Animation.enterPage(elem, null);
                    });
                });
            });
        },

        // binds the list to the listview
        bindUsers: function () {
            var self = this;
            hvData.getUsers(function (users) {
                // we are good so bind
                userList.winControl.itemDataSource = new WinJS.Binding.List(users).dataSource;
            },
            function () {
                // something went wrongshow a message to user and ask if they want to try again
                var msg = new Windows.UI.Popups.MessageDialog("Unable to get user list.");
                msg.commands.append(new Windows.UI.Popups.UICommand("Try again", function (command) {
                    WinJS.Promise.timeout(300).then(function () {
                        self.bindUsers();
                    });
                }));
                msg.commands.append(new Windows.UI.Popups.UICommand("Close"));
                msg.defaultCommandIndex = 0;
                msg.cancelCommandIndex = 1;
                msg.showAsync();
            });
        },

        // show settings to try and sign in
        showSettings: function () {
            WinJS.UI.SettingsFlyout.showSettings("login", "/pages/login/login.html");
        },

        status: {
            get: function () { return document.querySelector('section[role=main] p'); }
        },

        btnSignOut: {
            get: function () { return document.querySelector('section[role=main] button'); }
        }

    });
})();
