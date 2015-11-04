/**
 * Created by dmorina on 28/05/15.
 */

/**
 * WorkStationController
 * @namespace nextlap.factory.controllers
 */
(function () {
    'use strict';


    angular
        .module('nextlap.factory.controllers')
        .controller('WorkStationController', WorkStationController);

    WorkStationController.$inject = ['$location', '$scope', 'WorkStation', '$animate', '$window', '$routeParams', '$filter', 'DragAndDropAssignment', '$rootScope'];

    /**
     * @namespace WorkStationController
     */
    function WorkStationController($location, $scope, WorkStation, $animate, $window, $routeParams, $filter, DragAndDropAssignment, $rootScope) {
        console.debug('Creating WorkStationController');

        var self = this;
        self.params = $routeParams;
        self.workstations = [];
        self.addWorkStation = addWorkStation;
        self.removeWorkStation = removeWorkStation;
        self.getImage = getImage;
        self.getWorkStations = getWorkStations;
        self.name = null;
        self.droppableContainer = [];
        $scope.onOver = onOver;
        $scope.onDropWorkStation = onDropWorkStation;
        getWorkStations(self.params.stationId);
        self.currentStation = self.params.stationId;

        function onOver(event, ui) { // jshint ignore:line

        }

        function onDropWorkStation(event, ui) {
            var item = {
                data_id: $(ui.draggable).attr('data-id'),
                data_index: $(ui.draggable).attr('data-index'),
                data_type: $(ui.draggable).attr('data-type'),
                data_parent_id: $(ui.draggable).attr('data-parent-id'),
                data_parent_parent_id: $(ui.draggable).attr('data-parent-parent-id'),
                data_target: 'workstation'
            };
            var target = event.target.id;
            DragAndDropAssignment.moveToWorkStation(item, self.workstations[target].id).then(function success(data, status) { // jshint ignore:line
                $rootScope.$emit('itemDropped', {message: "OK", item: item, target: self.workstations[target]});
                WorkStation.getTaskCount(self.currentStation).then(function success(data, status) { // jshint ignore:line
                        var response_data = data.data;
                        self.workstations = self.workstations.map(function (obj) { 
                            var count = $filter('filter')(response_data, {id: obj.id})[0].cnt;
                            return angular.extend(obj, {task_count: count});
                        });
                    }, function error(data) {
                        $scope.notifyCtrl.notifyChange('Failed to load workstation task count: '+JSON.stringify(data.data));
                    }).finally(function () {
                    });
            }, function error(data) {
                $scope.notifyCtrl.notifyChange('Assignment to workstation failed: '+JSON.stringify(data.data));
            }).finally(function () {

            });
        }

        function getWorkStations(parent) {
            if (parent) {
                WorkStation.getWorkStations(parent).then(function success(data) {
                    self.workstations = data.data;
                    console.log('workstations', data.data);
                    WorkStation.getTaskCount(parent).then(function success(data) {
                        var response_data = data.data;
                        self.workstations = self.workstations.map(function (obj) {
                            var count = $filter('filter')(response_data, {id: obj.id})[0].cnt;
                            return angular.extend(obj, {task_count: count});
                        });
                    }, function error(data) {
                        $scope.notifyCtrl.notifyChange('Failed to load workstation task count: '+JSON.stringify(data.data));
                    }).finally(function () {
                    });
                }, function error(data) {
                   $scope.notifyCtrl.notifyChange('Failed to load workstations: '+JSON.stringify(data.data));
                }).finally(function () {
                    self.resetCreateForm();
                });
            }
        }

        function addWorkStation() {
            WorkStation.addWorkStation(self.currentStation).then(function success(data) {
                self.workstations.push(angular.extend(data.data, {task_count: 0}));
            }, function error(data) {
                $scope.notifyCtrl.notifyChange('Failed to add workstation: '+JSON.stringify(data.data));
            }).finally(function () {

            });
        }

        function removeWorkStation(index) {

            WorkStationController.removeWorkStation(self.stations[index].id).then(function success() {
                self.workstations.splice(index, 1);
            }, function error(data) {
                $scope.notifyCtrl.notifyChange('Failed to remove workstation: '+JSON.stringify(data.data));
            }).finally(function () {
            });
        }

        function getImage() {
            return '/static/images/material-design-icons/social/svg/production/ic_person_48px.svg';
        }

        $rootScope.$on('workstationChange', function (event, args) {
            self.currentStation = args.station;
            getWorkStations(args.station);
        });

        $scope.$on('$destroy', function() {
            console.debug('Destroying WorkStationController');
        });
    }
})();