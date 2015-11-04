(function() {
    'use strict';

    angular.module('nextlap.advanced.services', [])
        .factory('Advanced', Advanced);
    Advanced.$inject = ['$cookies', '$http', '$q','$rootScope'];
    function Advanced($cookies,$http, $q, $rootScope) {

        var Advanced = {
            toggleWIS: toggleWIS
        };
        return Advanced;

        function toggleWIS(id) {
            return $http({
                url: $rootScope.apiBaseUrl + '/stationtasks/toggleShowInWorkerScreen/'+id,
                method: 'POST'
            });
        }
    }
})();