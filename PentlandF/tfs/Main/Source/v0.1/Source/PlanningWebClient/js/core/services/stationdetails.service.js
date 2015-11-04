(function() {
    'use strict';

    angular.module('nextlap.core')
        .factory('StationDetails', StationDetailsService);

    StationDetailsService.$inject = ['$http', '$rootScope'];

    function StationDetailsService($http, $rootScope) {
        var services = {
            get: get
        };
        return services;

        function get(id) {
            return $http({
                url: $rootScope.apiBaseUrl + '/stationdetails/' + id,
                method: 'GET'
            });
        }
    }
})();