(function () {
    'use strict';

    angular.module('nextlap.core')
        .factory('Equipment', EquipmentService);

    EquipmentService.$inject = ['$http', '$rootScope'];

    function EquipmentService($http, $rootScope) {
        var services = {
            getTypes: getTypes,
            getAllEquipments: getAllEquipments,
            getEquipmentByStation: getEquipmentByStation,
            getEquipmentByWorkstation: getEquipmentByWorkstation,
            createEquipment: createEquipment,
            deleteEquipment: deleteEquipment,
            updateEquipment: updateEquipment,
            moveEquipment: moveEquipment,
            /* This is a subsection of the Equipment Service */
            Drivers: {
                get: getDrivers,
                getByType: getByType,
                create: createDriver
            }
        };
        return services;

        function getTypes(id) {
            return $http({
                url: $rootScope.apiBaseUrl + '/equipments/types',
                method: 'GET'
            });
        }
        function getAllEquipments(){
            return $http({
                url: $rootScope.apiBaseUrl + '/equipments',
                method: 'GET'
            });
        }
        function getEquipmentByStation(stationId) {
            return $http({
                url: $rootScope.apiBaseUrl + '/equipments/bystation/' + stationId,
                method: 'GET'
            });
        }

        function getEquipmentByWorkstation(workstationId) {
            return $http({
                url: $rootScope.apiBaseUrl + '/equipments/byworkstation/' + workstationId,
                method: 'GET'
            });
        }
        function createEquipment(equipment){
            return $http({
                url: $rootScope.apiBaseUrl + '/equipments/create',
                method: 'POST',
                data: {
                    name: equipment.name,
                    description: "",
                    equipmentTypeId: equipment.typeId,
                    workstationId: equipment.workstation_id,
                    config: equipment.config,
                    address: equipment.address,
                    driverId: equipment.driver_id
                }
            });

        }
        function updateEquipment(equipment){
            return $http({
                url: $rootScope.apiBaseUrl + '/equipments/update',
                method: 'POST',
                data: {
                    id: equipment.id,
                    name: equipment.name,
                    description: equipment.description,
                    //workstationId: equipment.workstation_id,
                    equipmentTypeId: equipment.typeId,
                    config: equipment.config,
                    address: equipment.address,
                    driverId: equipment.driver_id
                }
            });
        }

        function deleteEquipment(equipmentId){
            return $http({
                url: $rootScope.apiBaseUrl + '/equipments/delete/'+ equipmentId,
                method: 'DELETE'
            });
        }
        function moveEquipment(equipmentId, workstationId){
            return $http({
                url: $rootScope.apiBaseUrl + '/stationtasks/moveequipment',
                method: 'POST',
                data: {
                    id: equipmentId,
                    target_id: workstationId
                }
            });
        }
        // --------------------- BEGIN DRIVERS ------------------------

        function getDrivers() {
            return $http({
                url: $rootScope.apiBaseUrl + '/equipmentdrivers',
                method: 'GET'
            });
        }

        function getByType(typeId) {
            return $http({
                url: $rootScope.apiBaseUrl + '/equipmentdrivers/bytype/' + typeId,
                method: 'GET'
            });
        }

        function createDriver(name, description, typeId, clrType) {
           return $http({
                url: $rootScope.apiBaseUrl + '/equipmentdrivers/create',
                method: 'POST',
                data: {
                    name: name,
                    description: description,
                    typeId: typeId,
                    clrType: clrType
                }
            });
        }

        // --------------------- END DRIVERS ------------------------
    }
})();