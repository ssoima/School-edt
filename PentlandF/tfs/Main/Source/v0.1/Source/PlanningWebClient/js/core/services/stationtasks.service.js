(function() {
    'use strict';

    angular.module('nextlap.core')
        .factory('StationTask', StationTaskService);

    StationTaskService.$inject = ['$http', '$rootScope'];

    function StationTaskService($http, $rootScope) {
        var services = {
            getStationTasksForStation: getStationTasksForStation,
            getStationTasksForWorktation: getStationTasksForWorktation,
            assignEquipmentType: assignEquipmentType,
            assignEquipment: assignEquipment,
            assignEquipmentDriver: assignEquipmentDriver,
            moveWorkstationTask: moveWorkstationAssignment,
            unassignTask: unassignFromWorkstation,
            unassignWorkStation: unassignWorkStation,
            configureStationTaskAssignment: configureStationTaskAssignment
        };

        return services;

        function getStationTasksForStation(stationId) {
            return $http({
                url: $rootScope.apiBaseUrl + '/stationtasks/bystation/' + stationId,
                method: 'GET'
            });
        }

        function getStationTasksForWorktation(workstationId) {
            return $http({
                url: $rootScope.apiBaseUrl + '/stationtasks/byworkstation/' + workstationId,
                method: 'GET'
            });
        }

        function assignEquipmentType(stationTaskId, equipmentTypeId, overwrite) {
            var url = overwrite == undefined ? '/stationtasks/assignequipmenttype' : '/stationtasks/assignequipmenttype/' + (overwrite === true ? 'true' : 'false');
            return $http({
                url: $rootScope.apiBaseUrl + url,
                method: 'POST',
                data: {
                    id: stationTaskId,
                    target_id: equipmentTypeId
                }
            });
        }

        function assignEquipment(stationTaskId, equipmentId, overwrite) {
            var url = overwrite == undefined ? '/stationtasks/assignequipment' : '/stationtasks/assignequipment/' + (overwrite === true ? 'true' : 'false'); // bug true condition endpoint doesnt exist
            return $http({
                url: $rootScope.apiBaseUrl + url,
                method: 'POST',
                data: {
                    id: stationTaskId,
                    target_id: equipmentId
                }
            });
        }

        function moveWorkstationAssignment(stationTaskId, newPredecessorId) {
            console.log('moving ' + stationTaskId + ' new predecessor ' + newPredecessorId);
            return $http({
                url: $rootScope.apiBaseUrl + '/stationtasks/move',
                method: 'POST',
                data: {
                    id: stationTaskId,
                    lft: newPredecessorId
                }
            });
        }

        function unassignFromWorkstation(stationTaskId) {
            return $http({
                url: $rootScope.apiBaseUrl + '/stationtasks/delete/' + stationTaskId,
                method: 'DELETE'
            });
        }

        function assignEquipmentDriver(equipmentConfigurationId, equipmentDriverId) {
            return $http({
                url: $rootScope.apiBaseUrl + '/stationtasks/assigndriver',
                method: 'POST',
                data: {
                    id: equipmentConfigurationId,
                    target_id: equipmentDriverId
                }
            });
        }

        function unassignWorkStation(id){
            return $http({
                url: $rootScope.apiBaseUrl + '/stationtasks/unassigntaskfromworkstation',
                method: 'POST',
                data: {
                    id: id
                }
            });
        }
        function configureStationTaskAssignment(id, value){
            return $http({
                url: $rootScope.apiBaseUrl + '/stationtasks/configuredriver',
                method: 'POST',
                data: {
                    id: id,
                    values: [{key: 'cmd', val: value}]
                }
            });
        }
    }
})();