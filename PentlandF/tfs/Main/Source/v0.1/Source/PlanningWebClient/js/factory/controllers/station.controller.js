/* jshint loopfunc:true */ 
/**
 * Created by dmorina on 28/05/15.
 */
/**
 * Created by dmorina on 21/05/15.
 */
/**
* StationController
* @namespace nextlap.factory.controllers
*/
(function (_) {
    'use strict';

    angular
      .module('nextlap.factory.controllers')
      .controller('StationController', StationController);

    StationController.$inject = ['$location', '$scope', 'Station', '$animate', '$window', '$routeParams', '$filter', 'DragAndDropAssignment', '$rootScope', 'StationTask', 'debug'];

    /**
    * @namespace StationController
    */
    function StationController($location, $scope, Station, $animate, $window, $routeParams, $filter, DragAndDropAssignment, $rootScope, StationTask, debug) {
        var self = this;
        self.params = $routeParams;
        self.addStation = addStation;
        self.removeStation = removeStation;
        self.stationTypes = [];
        self.getImage = getImage;
        self.insertPosition = 'After';
        self.getStations = getStations;
        self.initialType = null;
        self.name = null;
        self.numberOfWorkers = 1;
        self.description = null;
        self.siblingStation = null;
        self.droppableContainer = [];
        self.isDroppable = isDroppable;
        self.showAdvancedStation = showAdvancedStation;
        self.activate = activate;
        $scope.onOver = onOver;
        $scope.onDrop = onDrop;
        getStations(self.params.sectionId);
        getStationTypes();

        function activate(){
            //temporary
            $rootScope.sectionId = $routeParams.sectionId;

        }
        self.activate();
        function onOver(event, ui) { // jshint ignore:line

        }
        function onDrop(event, ui) {
            var item = {
                data_id: $(ui.draggable).attr('data-id'),
                data_index: $(ui.draggable).attr('data-index'),
                data_type: $(ui.draggable).attr('data-type'),
                data_parent_id: $(ui.draggable).attr('data-parent-id'),
                data_target: 'station',
                data_count: $(ui.draggable).attr('data-task-count')
            };
            var target = event.target.id;
            //$scope.stations[target].task_count += parseInt(item.data_count);
            //TODO counters are not correct when reassigning tasks
            DragAndDropAssignment.moveToStation(item, $scope.stations[target].id).then(function success() {
                $rootScope.$emit('itemDropped', { message: "OK", item: item, target: $scope.stations[target] });
                Station.getTaskCount($routeParams.sectionId).then(function success(data) {
                    //$scope.stations = self.orderStations(data.data);
                    var response_data = data.data;
                    $scope.stations = $scope.stations.map(function(obj) {
                        var count = $filter('filter')(response_data, {id: obj.id})[0].cnt;
                        return angular.extend(obj, {task_count: count});
                    });
                }, function error(data) {
                    $scope.notifyCtrl.notifyChange('Assignment failed: '+JSON.stringify(data.data));
                }).finally(function () {
                });
            }, function error(data) {
                $scope.notifyCtrl.notifyChange('Assignment failed: '+JSON.stringify(data.data));
            });
        }
        function getStations(parent) {
            Station.getStations(parent).then(function success(data) {
                $scope.stations = self.orderStations(data.data);
                Station.getTaskCount(parent).then(function success(data) {
                    var response_data = data.data;
                    $scope.stations = $scope.stations.map(function(obj) {
                        var count = $filter('filter')(response_data, {id: obj.id})[0].cnt;
                        return angular.extend(obj, {task_count: count});
                    });
                }, function error(data, status) {
                    console.log("errors" + status);
                }).finally(function () {
                });
            }, function error() {
                $scope.notifyCtrl.notifyChange('Failed to load stations');
            }).finally(function () {
                self.resetCreateForm();
            });
        }
        function getStationTypes() {
            Station.getStationTypes().then(function success(data) {
                self.stationTypes = data.data;
            }, function error() {
                $scope.notifyCtrl.notifyChange('Failed to load station types');
            }).finally(function () {
                self.initialType = getType('Worker');
            });
        }

        function addStation() {
            if (!self.siblingStation){
                self.position_error = 'Please select a position';
                return;
            }
            var realPredecessorStationId = self.insertPosition === 'After' ? self.siblingStation.id : self.siblingStation.lft;
            Station.addStation(self.params.sectionId, realPredecessorStationId, self.name, self.description, self.initialType.name, self.numberOfWorkers).then(function success(data) {
                var pos = data.data.lft === null ? 0 : _.findIndex($scope.stations, function (item) {
                    return item.id === data.data.lft;
                }) + 1;
                var response_data = data.data;
                angular.extend(response_data, {task_count: 0});
                $scope.stations.splice(pos, 0, data.data);

            }, function error() {
                $scope.notifyCtrl.notifyChange('Failed to add station');
            }).finally(function () {
                self.resetCreateForm();
            });
        }
        function removeStation(index) {

            Station.removeStation($scope.stations[index].id).then(function success() {

                $scope.stations.splice(index, 1);
            }, function error() {
                $scope.notifyCtrl.notifyChange('Failed to remove station');
            }).finally(function () {
            });
        }
        function getImage(type) {
            if (type.match(/(person|default|worker|human)/gi)) {
                return '/static/images/material-design-icons/social/svg/production/ic_person_48px.svg';
                //return '/images/material-design-icons/action/svg/production/ic_supervisor_account_48px.svg';
            }
            else if (type.match(/(robot)/gi)) {
                return '/static/images/material-design-icons/action/svg/production/ic_android_48px.svg';
            }
            else if (type.match(/(buffer)/gi)) {
                return '/static/images/material-design-icons/action/svg/production/ic_schedule_48px.svg';
            }
            else {
                return 'default';
            }
        }
        function showAdvancedStation(stationId) {
            $scope.ctrl.navigateTo('station/'+stationId+'/sequence', true);
        }

        self.orderStations = function (stationsArray) {
            return _.sortBy(stationsArray, function (value, index, list) {
                return getPositionFromList(value, index, list);
            });
        };

        self.resetCreateForm = function () {
            this.insertPosition = 'After';
            // per default the target station is the last one
            this.siblingStation = this.stations.length === 0 ? null : this.stations[this.stations.length - 1];
            this.initalType = getType('Worker');
            this.description = null;
            this.numberOfWorkers = 2;
            this.name = null;
        };

        function getPositionFromList(value, currentIndex, list) {
            var c = value.lft,
                idx = 0;
            while (c !== null) {
                var next = _.find(list, function (item) { return item.id === c; });
                c = next ? next.lft : null;
                idx++;
            }
            return idx;
        }

        function getType(name) {
            return _.find(self.stationTypes, function (item) { return item.name === name; });
        }
        function isDroppable(type){
            return !!type.match(/(person|default|worker|human|robot)/gi);
        }
        
        debug && $scope.$on('$destroy', function() {
            console.debug('Destroying StationController');
        });
        debug &&console.debug('Creating StationController');
    }
})(_);