/*jshint loopfunc: true */
(function () {
    'use strict';

    angular.module('nextlap.advanced.controllers')
        .controller('AdvancedController', AdvancedController);

    AdvancedController.$inject = ['$scope', '$location', '$routeParams', 'StationDetails','Advanced', '$filter',
        '$rootScope', '$mdDialog',
        'StationTask', 'Station', '$q', '$timeout',
        'DragAndDropAssignment', 'WorkStation', 'Equipment', 'Upload', 'debug'];

    function AdvancedController($scope, $location, $routeParams, StationDetails, Advanced, $filter,
                                $rootScope, $mdDialog, StationTask, Station, $q,
                                $timeout, DragAndDropAssignment, WorkStation, Equipment, Upload, debug) {
        debug &&console.debug('Creating AdvancedController');
        var self = this;
        self.id = $routeParams.stationId;
        self.toggle = toggle;
        self.selectedItems = [];
        self.getSelectedItemsString = getSelectedItemsString;
        self.isSelected = isSelected;
        self.sort = sort;
        self.config = {
            order_by: "",
            order: ""
        };
        self.moveItem = moveItem;
        self.dragStart = dragStart;
        self.dragStop = dragStop;
        self.moveDown = moveDown;
        self.moveUp = moveUp;
        $scope.closeDialog = closeDialog;
        self.tasks = [];
        self.data = [];
        self.processIncomingData = processIncomingData;
        self.selectedWorkStation = {id: null};
        self.workStationToMoveTo = null;
        self.workerData = [];
        self.getViewingText = getViewingText;
        self.setSelectedWorkStation = setSelectedWorkStation;
        self.isWorkstationSelected = isWorkstationSelected;
        self.unAssignTask = unAssignTask;
        self.moveType = 'Station';
        self.workstations = loadWorkstations();
        self.selectedStation = null;
        self.searchText = null;
        self.querySearch = querySearch;
        self.simulateQuery = false;
        self.isDisabled = false;
        self.moveToStationOrWorkStation = moveToStationOrWorkStation;
        
        // Attach "callback" to activate: select the first workstation, if none is selected
        activate().then(function() {
            if(self.selectedWorkStation && self.selectedWorkStation.id === null) {
                self.setSelectedWorkStation(self.data.workers[0]);
            }
        });
        self.switchStation = switchStation;
        self.toggleWIS = toggleWIS;
        self.northWorkstations = [];
        self.southWorkstations = [];
        self.getTimeLapse = getTimeLapse;
        self.getEquipments = getEquipments;
        self.getNumberOfTasks = getNumberOfTasks;
        self.numberOfTasksMapping = [];
        self.switchStationTab = switchStationTab;
        self.openEquipmentSidenav = openEquipmentSidenav;
        self.equipmentTypes = [];
        self.equipmentDriverTypes = [];
        self.createEquipment = createEquipment;
        self.imageDialog = imageDialog;
        self.uploadImageFile = uploadImageFile;
        self.workstationEquipments = [];
        self.getTaskImageUrl = getTaskImageUrl;
        self.imageUrl = '';
        self.taskEquipmentConfiguration = taskEquipmentConfiguration;
        self.configureEquipment = configureEquipment;
        self.deleteEquipment = deleteEquipment;
        self.updateEquipment = updateEquipment;
        self.createOrUpdateEquipment = createOrUpdateEquipment;
        self.announceTypeChange = announceTypeChange;
        self.clearCreateForm = clearCreateForm;
        $scope.onDrop = onDrop;
        $scope.onDropToShelf = onDropToShelf;
        self.isCellFilled = isCellFilled;
        self.isShelf = Advanced.isShelf;
        self.equipment = {
            id: null,
            name: null,
            ipAddress: null,
            type: null,
            driver: null,
            rows: 3,
            columns: 3
        };
        self.isFirstStation = isFirstStation;
        self.isLastStation = isLastStation;
        self.selectAllModel = false;

        function resetEquipment() {
            self.equipment = {
                id: null,
                name: null,
                ipAddress: null,
                type: null,
                driver: null,
                rows: 3,
                columns: 3
            };
        }

        $rootScope.range = function (n) {
            return new Array(n);
        };
        function activate() {
            var deferred = $q.defer();
            StationDetails.get($routeParams.stationId).then(function success(data) {
                self.data = data.data;
                getEquipments();
                calculateNumberOfTasks();
                var workerTasks = $filter('filter')(self.data.assignedTasks, {workstationId: self.selectedWorkStation.id});
                processIncomingData(workerTasks);
                self.northWorkstations = $filter('filter')(self.data.workers, {side: 'left'});
                self.southWorkstations = $filter('filter')(self.data.workers, {side: 'right'});
                self.flexWidth = Math.floor(100 / (Math.max(self.northWorkstations.length, self.southWorkstations.length)));
                deferred.resolve();
            });

            Station.stations($routeParams.sectionId).then(function(response) {
                self.currentStation = $filter('filter')(response.data, { id: parseInt(getCurrentStation()) })[0].name;
                self.stationCount = response.data.length;
            });
            window.advancedController = self;
            window.advancedScope = $scope;
            return deferred.promise;
        }

        function toggle(item) {
            //var idx = self.selectedItems.indexOf(item);
            var filter = $filter('filter')(self.selectedItems, {id: item.id});
            if(filter.length) {
                self.selectedItems.splice(self.selectedItems.indexOf(filter[0]), 1);
            } else {
                self.selectedItems.push(item);
            }
            //item.isSelected = !item.isSelected;
        }




        function getSelectedItemsString() {
            var selectedCount = self.selectedItems.length;
            if (selectedCount === self.data.length) {
                return "All items selected";
            }
            else if (selectedCount === 1) {
                return "1 item selected";
            }
            else {
                return selectedCount + " items selected";
            }
        }

        function isSelected(item) {
            return !(self.selectedItems.indexOf(item) < 0); // jshint ignore:line
        }

        function sort(header) {
            var sortedData = $filter('orderBy')(self.workerData, header, self.config.order === 'descending');
            self.config.order = (self.config.order === 'descending') ? 'ascending' : 'descending';
            self.config.order_by = header;
            self.workerData = sortedData;
        }

        function dragStart(event, ui) {
            ui.item.data('start', ui.item.index());
        }

        function dragStop(event, ui) {
            var start = ui.item.data('start'),
                end = ui.item.index();
            applyMove(start, end);

            $scope.$apply();
        }

        function applyMove(startIndex, endIndex) {

            angular.forEach(self.workerData, function(item) {
                if(item.id === item.lft) {
                    console.log('lft and id identical', item);
                }
            });

            var old_lft = self.workerData[startIndex].lft;
            var new_lft = null;
            if (startIndex < endIndex) {
                new_lft = self.workerData[endIndex].id;
                self.workerData[startIndex].lft = new_lft;
                self.workerData[startIndex + 1].lft = old_lft;
                if (endIndex < self.workerData.length - 1) {
                    self.workerData[endIndex + 1].lft = self.workerData[startIndex].id;
                }
            }
            else {
                new_lft = self.workerData[endIndex].lft;
                self.workerData[startIndex].lft = new_lft;
                if (startIndex + 1 < self.workerData.length) {
                    self.workerData[startIndex + 1].lft = old_lft;
                }
                self.workerData[endIndex].lft = self.workerData[startIndex].id;
            }
            StationTask.moveWorkstationTask(self.workerData[startIndex].id, new_lft).then(function success(data, status) { // jshint ignore:line
                self.workerData.splice(endIndex, 0,
                    self.workerData.splice(startIndex, 1)[0]);
                addTimeLapse();
            }, function error(data, status) { // jshint ignore:line
            }).finally(function () {
            });
        }

        function moveItem($event) {
            var header = $scope.ctrl.stationTab === 'sequence' ? "Move part task to another station/workstation" : "Move equipment to another station/workstation";
            $("[ng-hide='$mdAutocompleteCtrl.hidden']").remove(); // Dirty hack to fix TFS Bug 2166, in Material-Design v0.11.4 this bug is already fixed
            $mdDialog.show({
                targetEvent: $event,
                scope: $scope,
                preserveScope: true,
                template: '<md-dialog>' +
                '  <md-dialog-content>' +
                '<h4>' + header + '</h4>' +
                '<form ng-submit="$event.preventDefault()" name="stationForm">' +
                '        <md-autocomplete flex required' +
                '            md-input-name="autocompleteField"' +
                '            md-no-cache="advanced.noCache"' +
                '            md-selected-item="advanced.workStationToMoveTo"' +
                '            md-search-text="advanced.searchText"' +
                '            md-items="item in advanced.querySearch(advanced.searchText)"' +
                '            md-item-text="item.station+ \' \' +item.name"' +
                '            md-floating-label="Work Station">' +
                '            <md-item-template>' +
                '                <span md-highlight-text="advanced.searchText">{{item.station +\' \' + item.name}}</span>' +
                '            </md-item-template>' +
                '            <div ng-messages="stationForm.autocompleteField.$error" ng-if="stationForm.autocompleteField.$touched && !advanced.workStationToMoveTo">' +
                '                <div ng-message="required">You <b>must</b> select a valid work station.</div>' +
                '            </div>' +
                '        </md-autocomplete>' +
                '</form>' +
                '</md-dialog-content>' +
                '  <div class="md-actions">' +
                '    <md-button ng-click="closeDialog()" class="md-primary">' +
                '      Cancel' +
                '    </md-button>' +
                '    <md-button ng-disabled="!advanced.workStationToMoveTo" ng-click="advanced.moveToStationOrWorkStation()" class="md-primary">' +
                '      Move' +
                '    </md-button>' +
                '  </div>' +
                '</md-dialog>',
                //controller: 'AdvancedController',
                onComplete: afterShowAnimation,
                locals: {tasks: self.selectedItems}
            });
            function afterShowAnimation(scope, element, options) { // jshint ignore:line
            }

            resetMoveForm();
        }

        function closeDialog() {
            self.searchText = undefined; // Clean up after closing dialog
            $mdDialog.hide();
            resetMoveForm();
            self.moveType = 'Station';
        }

        function moveDown() {
            angular.forEach(self.selectedItems, function (obj) {
                var index = self.workerData.indexOf(obj);
                if (index < self.workerData.length - 1) {
                    applyMove(index, index + 1);
                }
            });
        }

        function moveUp() {
            angular.forEach(self.selectedItems, function (obj) {
                var index = self.workerData.indexOf(obj);
                if (index > 0) {
                    applyMove(index, index - 1);
                }
            });
        }

        function processIncomingData(data) {
            self.workerData = insertionSort(data);
            addTimeLapse();
        }

        function addTimeLapse() {
            if(!self.workerData)
                return;
            self.workerData = self.workerData.map(function (obj, index) {
                var time_lapse;
                if (index > 0) {
                    var lapse = obj.time.split(':');
                    var lapse_acc = self.workerData[index - 1].time_lapse.split(':');
                    var d1 = new Date();
                    d1.setHours(parseInt(lapse[0]) + parseInt(lapse_acc[0]));
                    d1.setMinutes(parseInt(lapse[1]) + parseInt(lapse_acc[1]));
                    d1.setSeconds(parseInt(lapse[2]) + parseInt(lapse_acc[2]));
                    var h = addZero(d1.getHours());
                    var m = addZero(d1.getMinutes());
                    var s = addZero(d1.getSeconds());
                    time_lapse = h + ":" + m + ":" + s;
                }
                else {
                    time_lapse = obj.time;
                }

                return angular.extend(obj, {time_lapse: time_lapse});
            });
        }

        function insertionSort(data) {
            if(data.length === 0) {
                return [];
            }
            var allDone = false;
            var sortedArray = [];
            var counter = 0;
            while(!allDone && counter < 1500) {
                angular.forEach(data, function(task) {
                    if(sortedArray.length === 0 && typeof(task.lft) === 'object') {
                        sortedArray.push(task);
                    } else if(sortedArray.length > 0 && task.lft === sortedArray[sortedArray.length - 1].id) {
                        sortedArray.push(task);
                    }
                    if(sortedArray.length === data.length) {
                        allDone = true;
                    }

                });
                counter ++;
            }
            if(counter === 1500) {
                console.error('Sorting of tasks not possible (iterationlimit reached)', data);
                $scope.notifyCtrl.notifyChange('Failure: Tasklist from Backend not sortable, see console for details');
            }
            self.workerData = sortedArray;
            return sortedArray;
            /*var value, i, j;
            for (i = 0; i < data.length; i++) {
                value = data[i];
                for (j = i - 1; j > -1 && data[j].lft > value.lft; j--) {
                    data[j + 1] = data[j];
                    data[j + 1] = value;
                }
            }
            return data;*/
        }

        function addZero(i) {
            if (i < 10) {
                i = "0" + i;
            }
            return i;
        }

        function getViewingText() {
            if (self.selectedWorkStation.id === null) {
                return 'Viewing unassigned tasks';
            }
            else {
                return 'Viewing tasks assigned to workstation ' + self.selectedWorkStation.name;
            }
        }

        function setSelectedWorkStation(workStation) {
            self.selectedWorkStation = workStation;
            activate();
            resetSelectedItems();
        }

        function isWorkstationSelected(workStation) {
            return (self.selectedWorkStation.id === workStation.id);
        }

        function unAssignTask() {
            angular.forEach(self.selectedItems, function (obj) {
                if (self.selectedWorkStation.id !== null) {
                    StationTask.unassignWorkStation(obj.id).then(function success() {
                        removeFromResults(obj);
                        $filter('filter')(self.numberOfTasksMapping, { worker_id: self.selectedWorkStation.id })[0].number_of_tasks -= 1;
                        $filter('filter')(self.numberOfTasksMapping, { worker_id: null })[0].number_of_tasks += 1;
                        $scope.notifyCtrl.notifyChange('Task "' + obj.name + '" unassigned. (Moved to ' + obj.station + ' \/ unassigned)');
                        self.selectAllModel = false;
                    }, function error(data) {
                        $scope.notifyCtrl.notifyChange('Unassigning task failed: ' + JSON.stringify(data.data));
                    });
                }
                else {
                    StationTask.unassignTask(obj.id).then(function success() {
                        removeFromResults(obj);
                        $filter('filter')(self.numberOfTasksMapping, { worker_id: null })[0].number_of_tasks -= 1;
                        $scope.notifyCtrl.notifyChange('Task "' + obj.name + '" removed from ' + obj.station);
                        self.selectAllModel = false;
                    }, function error(data) {
                        $scope.notifyCtrl.notifyChange('Removing task from ' + obj.station + ' failed: ' + JSON.stringify(data.data));
                    });
                }

            });

        }

        function removeFromResults(obj) {
            var index = self.selectedItems.indexOf(obj);
            var index_1 = self.workerData.indexOf(obj);
            self.selectedItems.splice(index, 1);
            self.workerData.splice(index_1, 1);
        }

        function querySearch(query) {
            var results = query ? self.workstations.filter(createFilterFor(query)) : self.workstations,
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

        function createFilterFor(query) {
            var lowercaseQuery = angular.lowercase(query);
            return function filterFn(result) {
                var re = new RegExp(lowercaseQuery, 'gi');
                return ((result.station + ' ' + result.name).match(re)); //value
            };
        }

        function loadWorkstations() {
            WorkStation.getWorkStationsBySection($routeParams.sectionId)
                .then(function success(data, status) { // jshint ignore:line
                    self.workstations = data.data;
                },
                function error(data, status) { // jshint ignore:line

                }).finally(function () {

                }
            );
        }

        function moveToStationOrWorkStation() {
            angular.forEach(self.selectedItems, function (obj) {
                var item = {id: obj.id};
                var workstation_id = obj.workstationId;
                if ($scope.ctrl.stationTab === 'sequence') {
                    $filter('filter')(self.numberOfTasksMapping, {worker_id: workstation_id})[0].number_of_tasks -= 1;
                    // If the workstation is not included in the mapping yet: add it (Bug 2179)
                    if($filter('filter')(self.numberOfTasksMapping, {worker_id: self.workStationToMoveTo.id}).length) {
                        $filter('filter')(self.numberOfTasksMapping, {worker_id: self.workStationToMoveTo.id})[0].number_of_tasks += 1;
                    } else {
                        self.numberOfTasksMapping.push({
                            worker_id: self.workStationToMoveTo.id,
                            number_of_tasks: 1
                        });
                    }
                    var tmp = {
                        name: self.workStationToMoveTo.name,
                        station: self.workStationToMoveTo.station
                    };
                    DragAndDropAssignment.moveToWorkStation(item, self.workStationToMoveTo.id).then(function success() {
                        var index = self.workerData.indexOf(obj);
                        self.workerData.splice(index, 1);
                        $scope.notifyCtrl.notifyChange('Task "' + obj.name + '" moved to "' + tmp.station + '"\/"' + tmp.name + '"');
                    }, function error(data) {
                        $scope.notifyCtrl.notifyChange('Moving task "' + obj.name + '" to "' + tmp.station + '"\/"' + tmp.name + '" failed:' + JSON.stringify(data.data));
                    });
                }
                else {
                    moveEquipment();
                }

            });
            closeDialog();
            self.selectAllModel = false;
            self.selectedItems = [];

            resetMoveForm(); // TODO: When to call and when not to call. Still not analyzed
        }

        function resetMoveForm() {
            self.selectedStation = null;
            self.workStationToMoveTo = null;
            self.moveType = 'Station';
            //resetSelectedItems();
        }

        function resetSelectedItems() {
            self.selectedItems = [];
            angular.forEach(self.workerData, function(item) {
                item.isSelected = false;
            });
        }

        function getCurrentStation() {
            return $routeParams.stationId;
        }

        function switchStation(offset) {
            var stations = Station.stations($routeParams.sectionId).then(function(result) { // jshint ignore:line
                var currentStationObject = $filter('filter')(result.data, { id: parseInt(getCurrentStation()) })[0];
                var currentIndex = result.data.indexOf(currentStationObject);
                var nextIndex = currentIndex + offset;
                if (nextIndex < 0 || nextIndex >= result.data.length) return;
                var path = $location.path();
                var re = /station\/\d+/gi;
                $location.path(path.replace(re, 'station/' + result.data[nextIndex].id));
            });            
        }

        function isFirstStation() {
            return parseInt(getCurrentStation()) === 1;
        }

        function isLastStation() {
            return parseInt(getCurrentStation()) + 1 > self.stationCount;
        }

        function toggleWIS() {
            angular.forEach(self.selectedItems, function (obj) {
                Advanced.toggleWIS(obj.id).then(function success(data, status) { // jshint ignore:line
                    obj.showOnWorkerscreen = !obj.showOnWorkerscreen;
                    $scope.notifyCtrl.notifyChange('Success: WIS toggled for task ' + obj.name);
                }, function error(data, status) { // jshint ignore:line
                }).finally(function () {
                });
            });
        }

        function getTimeLapse(workstationId) {
            var workstation = $filter('filter')(self.data.assignedTasks, {workstationId: workstationId});
            var time_lapse = 0;
            angular.forEach(workstation, function (obj) {
                var lapse = obj.time.split(':');
                var d1 = new Date();
                d1.setHours(parseInt(lapse[0]));
                d1.setMinutes(parseInt(lapse[1]));
                d1.setSeconds(parseInt(lapse[2]));
                //var h = 60 * 60 * d1.getHours();
                var m = 60 * d1.getMinutes();
                var s = d1.getSeconds();
                time_lapse += m + s;
            });
            return time_lapse;
        }

        function getEquipments() {
            Equipment.getEquipmentByWorkstation(self.selectedWorkStation.id || 1).then(function success(data, status) { // jshint ignore:line
                self.workstationEquipments = data.data;
            }, function error(data, status) { // jshint ignore:line
            }).finally(function () {
            });
        }

        function getNumberOfTasks(workstationId) {
            var num = $filter('filter')(self.numberOfTasksMapping, {worker_id: workstationId});
            if (num.length) {
                return num[0].number_of_tasks;
            }
            return 0;
        }

        function calculateNumberOfTasks() {
            self.numberOfTasksMapping = [];
            self.numberOfTasksMapping.push({
                worker_id: null,
                number_of_tasks: $filter('filter')(self.data.assignedTasks, {workstationId: null}).length
            });
            angular.forEach(self.data.workers, function (obj) {
                var numberOfTasks = $filter('filter')(self.data.assignedTasks, {workstationId: obj.id}).length;
                self.numberOfTasksMapping.push({worker_id: obj.id, number_of_tasks: numberOfTasks});
            });
        }

        function switchStationTab(tab) {
            if(self.selectedWorkStation && self.data.workers && self.data.workers.length && self.selectedWorkStation.id === null) {
                self.setSelectedWorkStation(self.data.workers[0]);
            }
            $scope.ctrl.setStationTab(tab);
            resetSelectedItems();
        }

        function openEquipmentSidenav(action) {
            if (action === 'create') {
                resetEquipment();
            }
            else {
                self.equipment = {};
                var config = self.selectedItems[0].config ? self.selectedItems[0].config.split(':') : '0:0';
                self.equipment = {
                    id: self.selectedItems[0].id,
                    name: self.selectedItems[0].name,
                    ipAddress: self.selectedItems[0].address,
                    type: {id: self.selectedItems[0].typeId, name: self.selectedItems[0].type},
                    //driver: {id: self.selectedItems[0].driverId, name: self.selectedItems[0].driver},
                    rows: parseInt(config[0]),
                    columns: parseInt(config[1])
                };
                hideShowLeftSideNav();
            }

            Equipment.getTypes().then(function success(data, status) { // jshint ignore:line
                self.equipmentTypes = data.data;
            }, function error(data, status) { // jshint ignore:line
            }).finally(function () {
            });
            Equipment.Drivers.get().then(function success(data, status) { // jshint ignore:line
                self.equipmentDriverTypes = data.data;
            }, function error(data, status) { // jshint ignore:line
            }).finally(function () {
            });
            $scope.ctrl.toggleRight();
        }

        function imageDialog() {
            self.file = null;
            getTaskImageUrl('');
            $scope.ctrl.toggleRight();
            /*$mdDialog.show({
             targetEvent: $event,
             scope: $scope,
             preserveScope: true,
             template: '<div> <label>File:</label> <input type="file" ng-file-select="" ng-model="advanced.file" name="file" accept="image/*"> </div>',
             locals: { tasks: null }
             });*/
        }

        function uploadImageFile() {
            if (self.file) {
                //TODO fix on success preview
                Upload.upload({
                    url: $rootScope.apiBaseUrl + '/stationtasks/upload',
                    fields: {'id': self.selectedItems[0].id},
                    file: self.file
                }).progress(function (evt) {
                    var progressPercentage = parseInt(100.0 * evt.loaded / evt.total);
                    console.log('progress: ' + progressPercentage + '% ' + evt.config.file.name);
                }).success(function (data, status, headers, config) {
                    self.selectedItems[0].hasImage = true;
                    console.log('file ' + config.file.name + 'uploaded. Response: ' + data);
                    //self.imageUrl = '';
                    //$scope.$apply();
                    getTaskImageUrl('?' + new Date().getTime());
                    //getTaskImageUrl();
                    $scope.ctrl.toggleRight();

                }).error(function (data, status, headers, config) { // jshint ignore:line
                    console.log('error status: ' + status);
                });

            }
        }

        function createOrUpdateEquipment() {
            if (self.equipment.id !== null) {
                updateEquipment();
            }
            else {
                createEquipment();
            }
            resetSelectedItems();
        }

        function createEquipment() {
            var equipment = {
                name: self.equipment.name,
                description: "",
                workstation_id: self.selectedWorkStation.id,
                address: self.equipment.ipAddress,
                //driver_id: self.equipment.driver.id,
                type: self.equipment.type.name,
                typeId: self.equipment.type.id,
                config: self.equipment.rows + ':' + self.equipment.columns
            };
            Equipment.createEquipment(equipment).then(function success(data, status) { // jshint ignore:line
                self.workstationEquipments.push(data.data);
                $scope.notifyCtrl.notifyChange('Success: Equipment ' + equipment.name + ' created');
                resetEquipment();
                hideShowLeftSideNav();
            }, function error(data, status) { // jshint ignore:line
            }).finally(function () {

            });
        }

        function updateEquipment() {
            var equipment = {
                id: self.selectedItems[0].id,
                name: self.equipment.name,
                description: "",
                workstation_id: self.selectedWorkStation.id,
                address: self.equipment.ipAddress,
                //driver_id: self.equipment.driver.id,
                type: self.equipment.type.name,
                typeId: self.equipment.type.id,
                config: self.equipment.rows + ':' + self.equipment.columns
            };
            Equipment.updateEquipment(equipment).then(function success(data, status) { // jshint ignore:line
                var obj = $filter('filter')(self.workstationEquipments, {id: equipment.id})[0];
                var index = indexOfByProperty(self.workstationEquipments, obj);
                self.workstationEquipments[index] = data.data;
                $scope.notifyCtrl.notifyChange('Success: Equipment ' + equipment.name + ' updated');
                resetEquipment();
                hideShowLeftSideNav();
            }, function error(data, status) { // jshint ignore:line
            }).finally(function () {

            });
        }

        function indexOfByProperty(array, obj) {
            var i;
            for (i = 0; i < array.length; i++) {
                if (obj === array[i]) {
                    return i;
                }
            }
            return -1;
        }

        function deleteEquipment() {
            //deleteEquipment
            angular.forEach(self.selectedItems, function (obj) {
                Equipment.deleteEquipment(obj.id).then(function success(data, status) { // jshint ignore:line
                    self.workstationEquipments.pop(obj);
                    $scope.notifyCtrl.notifyChange('Success: Equipment ' + obj.name + ' deleted');
                }, function error(data, status) { // jshint ignore:line
                }).finally(function () {
                });
            });
        }

        function moveEquipment() {
            angular.forEach(self.selectedItems, function (obj) {
                Equipment.moveEquipment(obj.id, self.workStationToMoveTo.id).then(function success(data, status) { // jshint ignore:line
                    self.workstationEquipments.pop(obj);
                    $scope.notifyCtrl.notifyChange('Success: Equipment ' + obj.name + ' moved');
                }, function error(data, status) { // jshint ignore:line
                    $scope.notifyCtrl.notifyChange('Error: Could not move station - ' + data.data);
                }).finally(function () {
                });
            });
        }

        function getTaskImageUrl(randomPart) {
            if (self.selectedItems.length) {
                if (self.selectedItems[0].hasImage)
                    self.imageUrl = $rootScope.apiBaseUrl + '/stationtasks/image/' + self.selectedItems[0].id + randomPart;
                else
                    self.imageUrl = "";
            }

        }

        function taskEquipmentConfiguration($event) {
            $mdDialog.show({
                targetEvent: $event,
                scope: $scope,
                preserveScope: true,
                template: '<md-dialog>' +
                '  <md-dialog-content>' +
                '<h4>Configure task equipment</h4>' +
                '<md-select required ng-model="advanced.taskEquipment" placeholder="Equipment">' +
                '<md-select-label>{{advanced.taskEquipment ? advanced.taskEquipment.name : \'Select equipment\'}}</md-select-label>' +
                '<md-option ng-value="t" ng-repeat="t in advanced.workstationEquipments">{{t.name}}</md-option>' + '</md-select>' +
                '<!--md-input-container>' +
                '<label for="config">Configuration</label>' +
                '<input required type="text" id="config"' +
                'ng-model="advanced.taskEquipmentConfig">' +
                '</md-input-container-->' +
                '</md-dialog-content>' +
                '  <div class="md-actions">' +
                '    <md-button ng-click="closeDialog()" class="md-primary">' +
                '      Cancel' +
                '    </md-button>' +
                '    <md-button ng-disabled="!advanced.taskEquipment" ng-click="advanced.configureEquipment()" class="md-primary">' +
                '      Save' +
                '    </md-button>' +
                '  </div>' +
                '</md-dialog>',
                locals: {tasks: null}
            });
        }

        function configureEquipment() {
            angular.forEach(self.selectedItems, function (obj) {
                StationTask.assignEquipment(obj.id, self.taskEquipment.id, true).then(function success(data, status) { // jshint ignore:line
                    obj.equipment = self.taskEquipment.name;
                    obj.equipmentId = self.taskEquipment.id;
                }, function error(data, status) { // jshint ignore:line
                    $scope.notifyCtrl.notifyChange('Equipment assignment failed: ' + JSON.stringify(data.data));
                }).finally(function () {

                });

            });
            closeDialog();
        }

        function onDrop(event, ui) {
            if (!$(ui.draggable).attr('data-id'))
                return;
            if (self.selectedWorkStation.id === null) {
                var item = {
                    data_id: $(ui.draggable).attr('data-id'),
                    data_index: $(ui.draggable).attr('data-index'),
                    data_type: $(ui.draggable).attr('data-type'),
                    data_parent_id: $(ui.draggable).attr('data-parent-id'),
                    data_target: 'station',
                    data_count: $(ui.draggable).attr('data-task-count')
                };

                Station.stations($routeParams.sectionId).then(function success(data) {
                    self.stations = data.data;
                    var station = $filter('filter')(self.stations, {
                        id: $routeParams.stationId
                    }, function (actual, expected) {
                        return actual === parseInt(expected);
                    })[0];
                    DragAndDropAssignment.moveToStation(item, station.id).then(function success(data, status) { // jshint ignore:line
                        $rootScope.$emit('itemDropped', { message: "OK", item: item, target: station });
                    }).finally(function () {
                        activate();
                    });
                });
            }
            else {
                var item = { // jshint ignore:line
                    data_id: $(ui.draggable).attr('data-id'),
                    data_index: $(ui.draggable).attr('data-index'),
                    data_type: $(ui.draggable).attr('data-type'),
                    data_parent_id: $(ui.draggable).attr('data-parent-id'),
                    data_parent_parent_id: $(ui.draggable).attr('data-parent-parent-id'),
                    data_target: 'station',
                    assignment_type: 'direct'
                };

                Station.stations($routeParams.sectionId).then(function success(data) {
                    self.stations = data.data;
                    var station = $filter('filter')(self.stations, {
                        id: $routeParams.stationId
                    }, function (actual, expected) {
                        return actual === expected;
                    })[0];

                    DragAndDropAssignment.moveToWorkStation(item, self.selectedWorkStation.id).then(function success(data, status) { // jshint ignore:line
                        $rootScope.$emit('itemDropped', { message: "OK", item: item, target: station });
                    }, function error(data, status) { // jshint ignore:line
                        $scope.notifyCtrl.notifyChange('Error: ' + data.data);
                    }).finally(function () {
                        activate();
                    });
                });
            }
        }

        function announceTypeChange() {
            self.equipment.driver = null;
            hideShowLeftSideNav();
        }

        function hideShowLeftSideNav() {
            if (self.equipment.type) {
                $scope.ctrl.equipmentFormType = self.equipment.type.name;
                //$rootScope.isShelf = self.equipment.type.name.match(/shelf/gi);
            }
            else {
                $scope.ctrl.equipmentFormType = null;
            }

        }

        function clearCreateForm() {
            resetEquipment();
            hideShowLeftSideNav();
        }

        function onDropToShelf(event, ui) {
            var task_id = $(ui.draggable).attr('data-id');
            var value = $(event.target).attr('data-row') + ':' + $(event.target).attr('data-column');
            var obj = $filter('filter')(self.workerData, {id: task_id})[0];
            var task_index = indexOfByProperty(self.workerData, obj);
            StationTask.configureStationTaskAssignment(task_id, value).then(function success(data, status) { // jshint ignore:line
                self.workerData[task_index].equipmentDriver_config = value;
            }, function error(data, status) { // jshint ignore:line

            }).finally(function () {

            });
        }

        function isCellFilled(config) {
            return self.workerData && self.equipment ? $filter('filter')(self.workerData, {
                equipmentId: self.equipment.id,
                equipmentDriver_config: config,
                task: 'Pick'
            }).length : false;


        }

        $scope.$watch(function() { return self.selectAllModel; }, function(newValue) {
            angular.forEach(self.workerData, function(item) {
                if(newValue) {
                    item.isSelected = true;
                } else {
                    item.isSelected = false;
                }
            });
            if(newValue) {
                self.selectedItems = angular.copy(self.workerData);
            } else {
                self.selectedItems = [];
            }
        }, true);

        $scope.$on('$destroy', function() {
            $scope.ctrl.equipmentFormType = null; // Shows left menubar if it has been hidden in the past
        });

        debug && $scope.$on('$destroy', function() {
            console.debug('Destroying AdvancedController');
        });
        

    }
})();