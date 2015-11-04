/**
 * Authentication
 * @namespace nextlap.factory.services
 */
(function () {
    'use strict';
    angular
        .module('nextlap.factory.services')
        .factory('Cluster', Cluster);
    angular
        .module('nextlap.factory.services')
        .factory('Factory', Factory);
    angular
        .module('nextlap.factory.services')
        .factory('AssemblyLine', AssemblyLine);
    angular
        .module('nextlap.factory.services')
        .factory('Section', Section);
    angular
        .module('nextlap.factory.services')
        .factory('Cluster', Cluster);
    angular
        .module('nextlap.factory.services')
        .factory('Station', Station);
    angular
        .module('nextlap.factory.services')
        .factory('DragAndDropAssignment', DragAndDropAssignment);
    angular
        .module('nextlap.factory.services')
        .factory('WorkStation', WorkStation);
    Factory.$inject = ['$cookies', '$http', '$q', '$location', '$rootScope'];
    AssemblyLine.$inject = ['$cookies', '$http', '$q', '$location', '$rootScope'];
    Section.$inject = ['$cookies', '$http', '$q', '$location', '$rootScope'];
    Cluster.$inject = ['$cookies', '$http', '$q', '$location', '$rootScope'];
    Station.$inject = ['$cookies', '$http', '$q', '$location', '$rootScope'];
    DragAndDropAssignment.$inject = ['$cookies', '$http', '$q', '$location', '$rootScope'];
    WorkStation.$inject = ['$cookies', '$http', '$q', '$location', '$rootScope'];
    /**
     * @namespace Factory
     * @returns {Factory}
     */

    function Factory($cookies, $http, $q, $location, $rootScope) {
        /**
         * @name Factory
         * @desc The Factory to be returned
         */

        var self = this;
        self.factoryList = [];
        var Factory = {
            factories: getFactories,
            getCluster: getCluster
        };

        return Factory;

        ////////////////////

        /**
         * @name getFactory
         * @desc Try to get a factory
         * @returns {Promise}
         * @memberOf nextlap.factory.services.Factory
         */
        function getCluster(parent_id) {
            if ($rootScope.backend_type == 'DJANGO') {
                return $http({
                    //url: 'http://localhost:2283/factories/get/1',
                    url: '/api/cluster/by_parent/?parent=' + parent_id,
                    method: 'GET'

                });
            }
            else {
                return $http({
                    url: $rootScope.apiBaseUrl + '/factories/',
                    method: 'GET'
                });
            }
        }

        function getFactories() {
            var deferred = $q.defer();
            if (self.factoryList.length) return self.factoryList;
            $http({
                url: $rootScope.apiBaseUrl + '/factories/',
                method: 'GET'
            }).then(function success(response, status) {
                deferred.resolve(response.data);
            }, function error(data, status) {

            }).finally(function () {

            });
            self.factoryList = deferred.promise;
            return self.factoryList;

        }
    }

    function AssemblyLine($cookies, $http, $q, $location, $rootScope) {
        /**
         * @name AssemblyLine
         * @desc The Factory to be returned
         */
        var self = this;
        self.lines = [];
        var AssemblyLine = {
            assemblyLines: getAssemblyLinesDeferred,
            getAssemblyLines: getAssemblyLines
        };

        return AssemblyLine;

        ////////////////////

        /**
         * @name getFactory
         * @desc Try to get a factory
         * @returns {Promise}
         * @memberOf nextlap.factory.services.Factory
         */
        function getAssemblyLines(parent_id) {
            if ($rootScope.backend_type == 'DJANGO') {
                return $http({
                    url: '/api/cluster/by_parent/?parent=' + parent_id,
                    method: 'GET'
                });
            }
            else {
                return $http({
                    url: $rootScope.apiBaseUrl + '/assemblies/byFactory/get?factoryId=' + parent_id,
                    method: 'GET'
                });
            }
        }

        function getAssemblyLinesDeferred(parent_id) {
            var deferred = $q.defer();
            if (self.lines.length) return self.lines;
            $http({
                url: $rootScope.apiBaseUrl + '/assemblies/byFactory/get?factoryId=' + parent_id,
                method: 'GET'
            }).then(function success(response, status) {
                deferred.resolve(response.data);
            }, function error(data, status) {

            }).finally(function () {

            });
            self.lines = deferred.promise;
            return self.lines;
        }
    }

    function Section($cookies, $http, $q, $location, $rootScope) {
        /**
         * @name AssemblyLine
         * @desc The Factory to be returned
         */
        var self = this;
        self.sections = [];
        var Section = {
            sections: getSectionsDeferred,
            getSections: getSections
        };

        return Section;

        ////////////////////

        /**
         * @name getSections
         * @desc Try to get a factory
         * @returns {Promise}
         * @memberOf nextlap.factory.services.Factory
         */
        function getSections(parent_id) {
            if ($rootScope.backend_type == 'DJANGO') {
                return $http({
                    //url: 'http://localhost:2283/assemblies/byFactory/' + factory_id,
                    url: '/api/cluster/by_parent/?parent=' + parent_id,
                    method: 'GET'
                });
            }
            else {
                return $http({
                    url: $rootScope.apiBaseUrl + '/sections/byAssembly/get?assemblyLineId=' + parent_id,
                    method: 'GET'
                });
            }
        }
        function getSectionsDeferred(parent_id) {
            var deferred = $q.defer();
            if (self.sections.length) return self.sections;
            $http({
                url: $rootScope.apiBaseUrl + '/sections/byAssembly/get?assemblyLineId=' + parent_id,
                method: 'GET'
            }).then(function success(response, status) {
                deferred.resolve(response.data);
            }, function error(data, status) {

            }).finally(function () {

            });
            self.sections = deferred.promise;
            return self.sections;
        }
    }

    function Cluster($cookies, $http, $q, $location, $rootScope) {
        /**
         * @name AssemblyLine
         * @desc The Factory to be returned
         */
        var Cluster = {
            getClusters: getClusters,
            addCluster: addCluster
        };

        return Cluster;

        /**
         * @name getClusters
         * @desc Try to get clusters
         * @returns {Promise}
         * @memberOf nextlap.factory.services.Cluster
         */
        function getClusters(parent_id) {
            if ($rootScope.backend_type == 'DJANGO') {
                return $http({
                    url: '/api/cluster/by_parent/?parent=' + parent_id,
                    method: 'GET'
                });
            }
            else {
                return $http({
                    url: $rootScope.apiBaseUrl + '/sections/byAssembly/get?assemblyLineId=' + parent_id,
                    method: 'GET'
                });
            }
        }

        /**
         * @name addCluster
         * @desc Try to add a cluster
         * @returns {Promise}
         * @memberOf nextlap.factory.services.Cluster
         */
        function addCluster(parent, previous, next, name, description, type) {
            if ($rootScope.backend_type == 'DJANGO') {
                return $http({
                    url: '/api/cluster/',
                    method: 'POST',
                    data: {
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
                    url: $rootScope.apiBaseUrl + '/sections/byAssembly/get?assemblyLineId=' + parent_id,
                    method: 'GET'
                });
            }
        }
    }

    function Station($cookies, $http, $q, $location, $rootScope) {
        /**
         * @name AssemblyLine
         * @desc The Factory to be returned
         */
        var Station = {
            stations: getStationsDeferred,
            getStations: getStations,
            addStation: addStation,
            removeStation: removeStation,
            getStationTypes: getStationTypes,
            stations: getStations,
            getTaskCount: getTaskCount
        };

        return Station;

        /**
         * @name getStations
         * @desc Try to get stations
         * @returns {Promise}
         * @memberOf nextlap.factory.services.Cluster
         */
        function getStations(parent_id) {
            var promise = $q.defer();
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

        function getStationsDeferred(parent_id) {
            var deferred = $q.defer();
            if (self.stations.length) return self.stations;
            getStations(parent_id).then(function(result) {
                deferred.resolve(result);
            })
            self.stations = deferred.promise;
            return self.stations;
        }

        function getStationTypes() {
            if ($rootScope.backend_type == 'DJANGO1') { //TODO
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
                    data: {
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
                    data: {
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

        function getTaskCount(sectionId) {
            return $http({
                url: $rootScope.apiBaseUrl + '/stationtasks/count/bysection/' + sectionId,
                method: 'GET'
            });
        }

    }

    function DragAndDropAssignment($cookies, $http, $q, $location, $rootScope) {
        /**
         * @name DragAndDropAssigmnent
         * @desc The Factory to be returned
         */
        var DragAndDropAssignment = {
            moveToStation: moveToStation,
            moveToWorkStation: moveToWorkStation
        };

        return DragAndDropAssignment;

        /**
         * @name moveToStation
         * @desc Try to add parts to a station
         * @returns {Promise}
         * @memberOf nextlap.factory.services.DragAndDropAssignment
         */
        function moveToStation(item, target) {
            if ($rootScope.backend_type == 'DJANGO') {
                return $http({
                    url: '/api/cluster/',
                    method: 'POST',
                    data: {}
                });
            }
            else {
                var url = (item.data_type === 'part') ? $rootScope.apiBaseUrl + '/stationtasks/assignparttostation' :
                $rootScope.apiBaseUrl + '/stationtasks/assigntasktostation';
                return $http({
                    url: url,
                    method: 'POST',
                    data: {
                        id: item.data_id || item.id,
                        target_id: target
                    }
                });
            }
        }

        /**
         * @name moveToWorkStation
         * @desc Try to add parts to a station
         * @returns {Promise}
         * @memberOf nextlap.factory.services.DragAndDropAssignment
         */
        function moveToWorkStation(item, target) {
            var directAssignment = (item.assignment_type == 'direct') ? 'direct' : '';
            var url = (item.data_type === 'part') ? $rootScope.apiBaseUrl + '/stationtasks/assignparttoworkstation' :
            $rootScope.apiBaseUrl + '/stationtasks/' + directAssignment + 'assigntasktoworkstation';
            return $http({
                url: url,
                method: 'POST',
                data: {
                    id: item.data_id || item.id,
                    target_id: target
                }
            });
        }

    }

    function WorkStation($cookies, $http, $q, $location, $rootScope) {
        /**
         * @name WorkStation
         * @desc The Factory to be returned
         */
        var WorkStation = {
            getWorkStations: getWorkStations,
            getWorkStationsBySection: getWorkStationsBySection,
            addWorkStation: addWorkStation,
            removeWorkStation: removeWorkStation,
            getTaskCount: getTaskCount
        };

        return WorkStation;

        /**
         * @name getWorkStations
         * @desc Try to get stations
         * @returns {Promise}
         * @memberOf nextlap.factory.services.WorkStation
         */
        function getWorkStations(parent_id) {
            if ($rootScope.backend_type == 'DJANGO') {
                return $http({
                    url: '/api/cluster/by_parent/?parent=' + parent_id,
                    method: 'GET'
                });
            }
            else {
                return $http({
                    url: $rootScope.apiBaseUrl + '/workstations/byStation/get?stationId=' + parent_id,
                    method: 'GET'
                });
            }
        }

        /**
         * @name getWorkStationsBySection
         * @desc Try to get workstations by section id
         * @returns {Promise}
         * @memberOf nextlap.factory.services.WorkStation
         */
        function getWorkStationsBySection(section_id) {
            if ($rootScope.backend_type == 'DJANGO') {
                return $http({
                    url: '/api/cluster/by_parent/?parent=' + section_id,
                    method: 'GET'
                });
            }
            else {
                return $http({
                    url: $rootScope.apiBaseUrl + '/workstations/bySection/' + section_id,
                    method: 'GET'
                });
            }
        }

        /**
         * @name addWorkStation
         * @desc Try to add a station
         * @returns {Promise}
         * @memberOf nextlap.factory.services.WorkStation
         */
        function addWorkStation(parent) {
            if ($rootScope.backend_type == 'DJANGO') {
                return $http({
                    url: '/api/cluster/',
                    method: 'POST',
                    data: {
                        parent: parent
                    }
                });
            }
            else {
                return $http({
                    url: $rootScope.apiBaseUrl + '/workstations/createdefault',
                    method: 'POST',
                    data: {
                        stationId: parent
                    }
                });
            }
        }

        /**
         * @name removeWorkStation
         * @desc Try to delete a work station
         * @returns {Promise}
         * @memberOf nextlap.factory.services.WorkStation
         */
        function removeWorkStation(id) {
            if ($rootScope.backend_type == 'DJANGO') {
                return $http({
                    url: '/api/cluster/',
                    method: 'POST',
                    data: {
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
                    url: $rootScope.apiBaseUrl + '/workstations/delete/' + id,
                    method: 'DELETE'
                });
            }
        }

        function getTaskCount(stationId) {
            return $http({
                url: $rootScope.apiBaseUrl + '/stationtasks/count/bystation/' + stationId,
                method: 'GET'
            });
        }
    }
})();