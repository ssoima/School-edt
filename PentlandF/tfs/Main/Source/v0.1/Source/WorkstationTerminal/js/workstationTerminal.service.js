(function($) {
    angular.module('workstationTerminal')
        .factory('TerminalHubProxy', TerminalHubProxy);

    TerminalHubProxy.$inject = ['$rootScope', '$routeParams', 'signalrUrl'];

    function TerminalHubProxy($rootScope, $routeParams, signalrUrl) {

        var connection = $.hubConnection(signalrUrl);
        var proxy = connection.createHubProxy('terminal');
        connection.qs = { workstationId: $routeParams.wsid };

        return {
            start: function() {
                return connection.start();
            },
            on: function(eventName, callback) {
                proxy.on(eventName, function(result) {
                    $rootScope.$apply(function() {
                        if (callback) callback(result);
                    });
                });
            },
            invoke: function (methodName, callback) {
                var args = $.makeArray($.makeArray(arguments).slice(2), [methodName]);

                proxy.invoke.apply(proxy, args).done(function(result) {
                    $rootScope.$apply(function() {
                        if (callback) callback(result);
                    });
                });
            }
        };
    }

})(jQuery);