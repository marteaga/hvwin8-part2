// For an introduction to the Page Control template, see the following documentation:
// http://go.microsoft.com/fwlink/?LinkId=232511
(function () {
    "use strict";
    var self;

    WinJS.UI.Pages.define("/pages/userEntries/entries.html", {
        // This function is called whenever a user navigates to this page. It
        // populates the page elements with the app's data.
        ready: function (element, options) {
            // attempt to get user data
            this.getUserData();
        },

        // gets the user data from the system
        getUserData: function () {
            Application.healthVaultData.getUserEntries(Application.selectedUser.Id, function (data) {
                // we are good
                entries.winControl.itemDataSource = new WinJS.Binding.List(data).dataSource;
            },
            function () {
                // something went wrongshow a message to user and ask if they want to try again
                var msg = new Windows.UI.Popups.MessageDialog("Unable to get user data.");
                msg.commands.append(new Windows.UI.Popups.UICommand("Try again", function (command) {
                    WinJS.Promise.timeout(300).then(function () {
                        self.getUserData();
                    });
                }));
                msg.commands.append(new Windows.UI.Popups.UICommand("Close"));
                msg.defaultCommandIndex = 0;
                msg.cancelCommandIndex = 1;
                msg.showAsync();
            });
        },

        updateLayout: function (element, viewState, lastViewState) {
            /// <param name="element" domElement="true" />
            /// <param name="viewState" value="Windows.UI.ViewManagement.ApplicationViewState" />
            /// <param name="lastViewState" value="Windows.UI.ViewManagement.ApplicationViewState" />

            // TODO: Respond to changes in viewState.
        },

        unload: function () {
            // TODO: Respond to navigations away from this page.
        }
    });
})();
