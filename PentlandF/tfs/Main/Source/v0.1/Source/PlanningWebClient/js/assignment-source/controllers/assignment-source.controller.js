/**
 * Created by dmorina on 21/05/15.
 */
/**
 * LeftSearchController
 * @namespace nextlap.search.controllers
 */
(function () {
    'use strict';

    angular
        .module('nextlap.assignment-source.controllers')
        .controller('LeftSearchController', LeftSearchController);

    LeftSearchController.$inject = ['$location', '$scope', '$log', '$q', '$timeout', '$mdSidenav', '$mdUtil', 'LeftSearch', '$rootScope', '$filter', '$routeParams', 'Equipment'];
    /**
     * @namespace LeftSearchController
     */
    function LeftSearchController($location, $scope, $log, $q, $timeout, $mdSidenav, $mdUtil, LeftSearch, $rootScope, $filter, $routeParams, Equipment) {
        var self = this;
        self.simulateQuery = true;
        self.isDisabled = false;
        self.results = loadAll();
        self.querySearch = querySearch;
        $scope.uniqueOfProperty = uniqueOfProperty;
        self.countOfProperty = countOfProperty;
        self.selectedItemChange = selectedItemChange;
        self.searchTextChange = searchTextChange;
        self.filteredResults = [];
        self.workstationResults = [];
        $scope.drag = drag;
        $scope.onOver = onOver;
        self.unassignedOnly = false;
        self.workType = 'stations';
        self.navigate = navigate;
        self.activate = activate;
        self.getPartOrangeClass = getPartOrangeClass;
        self.getStepName = getStepName;
        self.isStepDone = isStepDone;
        self.notifyWorkStationChange = notifyWorkStationChange;
        self.equipmentTypes = getEquipmentTypes();
        self.getRootNodeStatus = getRootNodeStatus;
        function activate() {
            self.workType = !$location.path().match(/workstation/gi) ? 'stations' : 'workstations';
            // TODO change this when equipment is added
        }

        self.activate();
        $scope.$watch($scope.ctrl.stepId, function (oldval, newval) {
            if (oldval !== newval) {
                self.filteredResults = [];
                self.activate();
            }
        });
        function querySearch(query) {
            var results = query ? self.results.filter(createFilterFor(query)) : self.results,
                deferred;
            self.filteredResults = results;
            if (self.simulateQuery) {
                deferred = $q.defer();
                $timeout(function () {
                    deferred.resolve(results);
                }, Math.random() * 1000, false);
                return deferred.promise;
            } else {
                return results;
            }
        }

        function searchTextChange() {
        }

        function selectedItemChange() {
        }

        function drag() {
        }

        function onOver() {
        }

        function navigate(path) {
            var goTo = '';
            if (!self.sectionId) {
                self.sectionId = $routeParams.sectionId;
            }

            if (path) {
                $location.path(path);
            }
            else {
                goTo = (self.workType === 'stations') ? self.workType + '/' + self.sectionId : self.workType;
                $location.path(goTo);
                loadAll(); //TODO
            }

        }

        function uniqueOfProperty(list, field) {
            var unique = _.chain(list)
                .pluck(field)
                .flatten()
                .unique()
                .value();
            return _.sortBy(unique).join(', ').replace(/(^,)|(, $)/g, "");
        }

        function countOfProperty(list, field) {
            var count = _.chain(list)
                .pluck(field)
                .flatten()
                .compact()
                .value();
            return count;
        }

        $rootScope.$on('itemDropped', function (event, args) {

            if (args.item.data_type === 'part') {
                if (args.item.data_target === 'station') {
                    console.log('dropped part onto station');
                    var part_item = self.filteredResults[args.item.data_index];
                    //console.log(args);
                    $scope.notifyCtrl.notifyChange('Success: ' + args.item.data_type + ' ' + part_item.name + ' (' + part_item.number + ') assigned to ' +
                        args.target.name);
                    var part = self.filteredResults[args.item.data_index];
                    part.status = 'complete';
                    var partResult = $filter('filter')(self.results, {id: args.item.data_id})[0];
                    partResult.status = 'complete';
                    _.each(part.tasks, function (obj) {
                        obj.stationId = args.target.id;
                        obj.station = args.target.name;
                    });
                    _.each(partResult.tasks, function (obj) {
                        obj.stationId = args.target.id;
                        obj.station = args.target.name;
                    });
                }
                else if (args.item.data_target === 'workstation') {
                    console.log('dropped part onto workstation');
                    var part_item = $filter('filter')(self.filteredResults, {id: args.item.data_parent_id})[0].parts[args.item.data_index]; // jshint ignore:line
                    $scope.notifyCtrl.notifyChange('Success: ' + args.item.data_type + ' ' + part_item.name + ' (' + part_item.number + ') assigned to ' +
                        args.target.name);
                    var stationFiltered = $filter('filter')(self.filteredResults, {id: args.item.data_parent_id})[0];
                    var partFiltered = stationFiltered.parts[args.item.data_index];
                    partFiltered.remaining_tasks = 0;
                    _.each(partFiltered.tasks, function (obj) {
                        obj.workstation_id = args.target.id;
                        obj.workstation_name = args.target.name;
                    });
                }
            }
            else if (args.item.data_type === 'task') {

                if (args.item.data_target === 'station') {
                    console.log('dropped task onto station');
                    $scope.notifyCtrl.notifyChange('Success: ' + args.item.data_type + ' ' +
                        $filter('filter')(self.filteredResults, {id: args.item.data_parent_id})[0].tasks[args.item.data_index].name + ' assigned to ' +
                        args.target.name);
                    var parentFiltered = $filter('filter')(self.filteredResults, {id: args.item.data_parent_id})[0];
                    var parentResult = $filter('filter')(self.results, {id: args.item.data_parent_id})[0];
                    var taskFiltered = parentFiltered.tasks[args.item.data_index];
                    var taskResult = parentResult.tasks[args.item.data_index];
                    taskFiltered.isAssigned = true;
                    taskFiltered.stationId = args.target.id;
                    taskResult.isAssigned = true;
                    taskResult.stationId = args.target.id;
                    taskResult.station = args.target.name;
                    var assignedFilteredTasks = $filter('filter')(parentFiltered.tasks, {isAssigned: true});
                    if (assignedFilteredTasks.length === parentFiltered.tasks.length) {
                        parentFiltered.status = 'complete';
                        parentResult.status = 'complete';
                    }
                    else {
                        parentFiltered.status = 'partial';
                        parentResult.status = 'partial';
                    }
                }
                else if (args.item.data_target === 'directworkstation') {
                    console.log('dropped task onto directworkstation');

                }
                else {
                    console.log('dropped task onto else');
                    //$scope.notifyCtrl.notifyChange('Success: ' + args.item.data_type + ' ' +
                    //var station+$filter('filter')(self.filteredResults, {id: args.item.data_parent_parent_id})[0]);//.tasks[args.item.data_index].name + ' assigned to '
                    //+ args.item.data_target + ' ' + args.target.id);
                    var station = $filter('filter')(self.filteredResults, {id: args.item.data_parent_parent_id})[0];
                    var taskPart = $filter('filter')(station.parts, {id: args.item.data_parent_id})[0];
                    var task = taskPart.tasks[args.item.data_index];
                    $scope.notifyCtrl.notifyChange('Success: ' + args.item.data_type + ' ' + task.name + ' assigned to ' + args.target.name);
                    if (args.item.data_target === 'workstation') {
                        if (!task.workstation_id) taskPart.remaining_tasks -= 1;
                        task.workstation_id = args.target.id;
                        task.workstation_name = args.target.name;
                    }
                    else {
                        if (!task.equipment_id) taskPart.remaining_tasks -= 1;
                        task.equipment_id = args.target.id;
                        task.equipment_name = args.target.name;
                    }
                }
            }

        });
        function loadAll() {
            LeftSearch.loadPartTasks()
                .then(function success(data) {
                    self.results = data.data;
                    //console.log('rawResults', self.results);
                    var flatResults = [];
                    if ($scope.ctrl.stepId !== 1) {
                        angular.forEach(self.results, function (obj) {
                            var objTasks = $scope.ctrl.stepId === 2 ? obj.tasks : $filter('filter')(obj.tasks, {equipmentRequired: true});
                            angular.forEach(objTasks, function (innerObj) {
                                if (innerObj.stationId) {
                                    flatResults.push({
                                        part_id: obj.id,
                                        part_number: obj.number,
                                        part_name: obj.name,
                                        task_id: innerObj.currentAssignmentId,
                                        task_name: innerObj.name,
                                        station_id: innerObj.stationId,
                                        station_name: innerObj.station,
                                        workstation_id: innerObj.workstationId,
                                        workstation_name: innerObj.workstation,
                                        equipment_id: innerObj.equipmentId,
                                        equipment_name: innerObj.equipment,
                                        equipmentRequired: innerObj.equipmentRequired
                                    });
                                }
                            });
                        });
                        var groupedFlatResults = _.chain(flatResults).groupBy('station_id')
                            .map(function (value) {
                                return {
                                    id: value[0].station_id,
                                    name: value[0].station_name,
                                    status: 'nothing',
                                    parts: _.chain(value).groupBy('part_id').map(function (v) {
                                        return {
                                            id: v[0].part_id,
                                            name: v[0].part_name,
                                            number: v[0].part_number,
                                            tasks: _.chain(v).groupBy('task_id').map(function (innerValue) {
                                                return {
                                                    id: innerValue[0].task_id,
                                                    name: innerValue[0].task_name,
                                                    workstation_id: innerValue[0].workstation_id,
                                                    workstation_name: innerValue[0].workstation_name,
                                                    equipment_id: innerValue[0].equipment_id,
                                                    equipment_name: innerValue[0].equipment_name,
                                                    equipmentRequired: innerValue[0].equipmentRequired
                                                };
                                            }).value()
                                        };
                                    }).value()
                                };
                            })
                            .value();
                        angular.forEach(groupedFlatResults, function (obj) {
                            angular.forEach(obj.parts, function (partObj) {
                                var total = 0;
                                var remaining = 0;

                                if ($scope.ctrl.stepId === 2) {
                                    remaining = $filter('filter')(flatResults, {
                                        part_id: partObj.id,
                                        station_id: obj.id,
                                        workstation_id: null
                                    }, true).length;
                                    total = $filter('filter')(flatResults, {
                                        part_id: partObj.id,
                                        station_id: obj.id
                                    }, true).length;
                                }
                                else {
                                    remaining = $filter('filter')(flatResults, {
                                        part_id: partObj.id,
                                        station_id: obj.id,
                                        equipment_id: null,
                                        equipmentRequired: true
                                    }, true).length;
                                    total = $filter('filter')(flatResults, {
                                        part_id: partObj.id,
                                        station_id: obj.id,
                                        equipmentRequired: true
                                    }, true).length;

                                }


                                angular.extend(partObj, {total_tasks: total});
                                angular.extend(partObj, {remaining_tasks: remaining});
                            });

                        });
                        self.results = groupedFlatResults;
                        self.filteredResults = self.results;
                        //console.log(self.filteredResults);
                    }
                    else {
                        self.filteredResults = self.results;
                    }
                    //console.log('filteredResults', self.results);
                },
                function error() {

                }).finally(function () {

                }
            );
        }

        function createFilterFor(query) {
            var lowercaseQuery = angular.lowercase(query);
            return function filterFn(result) {
                var re = new RegExp(lowercaseQuery, 'gi');
                if (result.hasOwnProperty('number') && result.hasOwnProperty('name')) {
                    return (result.number.match(re) || result.name.match(re));
                }
                else if (result.hasOwnProperty('parts') && result.hasOwnProperty('name')) {
                    if (!result.name.match(re)) {
                        var i;
                        for (i = 0; i < result.parts.length; i++) {
                            if (result.parts[i].number.match(re) || result.parts[i].name.match(re)) {
                                return true;
                            }
                        }
                        return false;
                    }
                    return true;
                }
            };
        }

        self.close = function () {
            $mdSidenav('left').close()
                .then(function () {

                });
        };
        function getPartOrangeClass(tasks, property) {
            var workstationCount = self.countOfProperty(tasks, property).length;
            return !!(workstationCount > 0 && workstationCount < tasks.length);

        }

        function getStepName() {
            var path = $location.path();
            if (path.match(/workstations/gi)) {
                return 'Tasks';
            }
            else if (path.match(/equipments/gi)) {
                return 'Tasks';
            }
            else {
                return 'Tasks';
            }
        }

        $rootScope.$on('stepChanged', function () {
            loadAll();
        });
        function isStepDone() {
            if ($scope.ctrl.stepId === 1) {
                return true;
                //return $filter('filter', {status: "complete"}).length == self.filteredResults.length;
            }
        }

        function notifyWorkStationChange(station) {
            var expanded = $filter('filter')(self.filteredResults, {is_expanded: true}, true);
            angular.forEach(expanded, function (obj) {
                if (obj.id !== station)
                    obj.is_expanded = false;
            });
            $rootScope.$emit('workstationChange', {station: station});
        }

        function getEquipmentTypes() {
            Equipment.getTypes(null).then(function success(data) {
                self.equipmentTypes = data.data;
            }, function error() {
            }).finally(function () {

            });
        }

        function getRootNodeStatus(item) {
            var remaining = $filter('filter')(item.parts, {remaining_tasks: 0}, true);
            if (!remaining) return false;
            if (remaining.length === item.parts.length) {
                return 'complete';
            }
            else if (remaining.length < item.parts.length && remaining.length > 0) {
                return 'partial';
            }
            else {
                return "nothing";
            }
        }
    }
})();