(function() {
    'use strict';

    angular.module('nextlap.core')
        .factory('Station', StationService);

    StationService.$inject = ['$http', '$rootScope', 'EntityBase'];

    function StationService($http, $rootScope, EntityBase) {

        var services = Object.create(EntityBase);
        services.setEndPoint('/stations');
        return services;

        /**
        * @name getStations
        * @desc Try to get stations
        * @returns {Promise}
        * @memberOf nextlap.factory.services.Cluster
        */
        function getStations(parent_id) {
            if ($rootScope.backend_type == 'DJANGO') {
                return $http({
                    url: '/api/cluster/by_parent/?parent=' + parent_id,
                    method: 'GET'
                });
            }
            else {
                return $http({
                    url: $rootScope.apiBaseUrl + '/stations/bySection/get?sectionId=' + parent_id,
                    method: 'GET'
                });
            }
        }
        function getStationTypes() {
            if ($rootScope.backend_type == 'DJANGO') {
                return $http({
                    url: '/api/cluster/by_parent/?parent=' + parent_id,
                    method: 'GET'
                });
            }
            else {
                return $http({
                    url: $rootScope.apiBaseUrl + '/stations/types',
                    method: 'GET'
                });
            }
        }
        /**
        * @name addStation
        * @desc Try to add a station
        * @returns {Promise}
        * @memberOf nextlap.factory.services.Station
        */
        function addStation(parent, previous, name, description, type, worker) {
            if ($rootScope.backend_type == 'DJANGO') {
                return $http({
                    url: '/api/cluster/',
                    method: 'POST',
                    data:
                    {
                        parent: parent,
                        lft: previous,
                        rght: next,
                        name: name,
                        description: description,
                        type: type
                    }
                });
            }
            else {
                return $http({
                    url: $rootScope.apiBaseUrl + '/stations/createwithworker',
                    method: 'POST',
                    data: {
                        section: parent,
                        lft: previous,
                        name: name,
                        description: description,
                        type: type,
                        worker: worker
                    }
                });
            }
        }
        /**
       * @name removeStation
       * @desc Try to delete a station
       * @returns {Promise}
       * @memberOf nextlap.factory.services.Station
       */
        function removeStation(id) {
            if ($rootScope.backend_type == 'DJANGO') {
                return $http({
                    url: '/api/cluster/',
                    method: 'POST',
                    data:
                    {
                        parent: parent,
                        lft: previous,
                        rght: next,
                        name: name,
                        description: description,
                        type: type
                    }
                });
            }
            else {
                return $http({
                    url: $rootScope.apiBaseUrl + '/stations/delete/' + id,
                    method: 'DELETE'
                });
            }
        }
    }
})();