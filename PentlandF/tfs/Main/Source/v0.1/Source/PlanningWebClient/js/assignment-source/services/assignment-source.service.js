/**
* Authentication
* @namespace nextlap.search.services
*/
(function () {
    'use strict';
    angular
      .module('nextlap.assignment-source.services')
      .factory('LeftSearch', LeftSearch);


    LeftSearch.$inject = ['$cookies', '$http', '$q', '$location','$rootScope'];

    function LeftSearch($cookies, $http, $q, $location,$rootScope) {
        /**
        * @name AssemblyLine
        * @desc The Factory to be returned
        */
        var LeftSearch = {
            loadPartTasks: loadPartTasks
        };

        return LeftSearch;

        /**
        * @name loadPartTasks
        * @desc Search
        * @returns {Promise}
        * @memberOf nextlap.search.services.LeftSearch
        */
        function loadPartTasks() {
            if ($rootScope.backend_type=='DJANGO') { //TODO
                return $http({
                    url: '/api/tart/',
                    method: 'GET'
                });
            }
            else {
                return $http({
                    url: $rootScope.apiBaseUrl + '/assignableparts',
                    method: 'GET'
                });
            }
        }

    }

})();