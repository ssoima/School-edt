(function ($, _) {
    angular.module('workstationTerminal')
        .controller('CurrentTasksController', CurrentTasksController);

    CurrentTasksController.$inject = ['$scope', '$routeParams', '$interval', '$mdDialog', 'TerminalHubProxy', 'imageProviderBaseUrl'];

    function CurrentTasksController($scope, $routeParams, $interval, $mdDialog, TerminalHubProxy, imageProviderBaseUrl) {
        $scope.tasks = [];
        $scope.navigationSequence = [];
        $scope.currentOrderProgress = 0;
        $scope.currentOrder = '';
        $scope.currentModel = '';
        $scope.currentVariant = '';
        $scope.workstation = '';
        $scope.workstationId = $routeParams.wsid;
        $scope.imageBaseUrl = imageProviderBaseUrl;
        $scope.conveyanceBeltRunning = false;
        $scope.toggleConveyanceBeltRunStatus = toggleConveyanceBeltRunStatus;
        $scope.selectedItem = null;
        $scope.selectUp = selectUp;
        $scope.selectDown = selectDown;
        $scope.showSelectedImage = showSelectedImage;
        $scope.writeCommentToSelected = writeCommentToSelected;
        // we will store some metadata in 'that'
        var that = this;

        // register callbacks
        TerminalHubProxy.on('notifyOrderInStation', function (data) {
            // save current processing order as metadata
            that.currentOrder = data;
            // reset progress bar
            if (that.workstationInfoLoaded === true) {
                var stationOffset = that.workstationInfo.offset;
                var stationLength = that.workstationInfo.len;
                // the order position is the logical position on the line
                var currentOrderPosition = data.pos;
                // currentOrderPosition should be higher
                // get 0..100 in relation to offset, length and current position
                $scope.currentOrderProgress = ((currentOrderPosition - stationOffset) * 100) / (stationLength);
            } else {
                $scope.currentOrderProgress = 0;
            }
            $scope.navigationSequence = data.sequence;
            $scope.currentOrder = data.current;
            $scope.currentModel = data.model;
            $scope.currentVariant = data.variant;
            populateTasks(data.tasks);
        });
        TerminalHubProxy.on('notifyTaskProgress', function (data) {
            console.log(data.suc);
        });
        TerminalHubProxy.on('notifyWorkstationInfo', function(data) {
            that.workstationInfo = data;
            that.workstationInfoLoaded = true;
            // populate scope
            $scope.workstation = data.workstation + ' / ' + data.station;
        });
        TerminalHubProxy.on('notifyConveyanceStarted', function(model) {
            console.log('conveyance started');
            $scope.conveyanceBeltRunning = true;
        });
        TerminalHubProxy.on('notifyConveyanceStopped', function (model) {
            console.log('conveyance stopped');
            $scope.conveyanceBeltRunning = false;
        });
        TerminalHubProxy.on('notifyConveyanceProgress', function (model) {
            // and flag that the conveyance belt is running
            $scope.conveyanceBeltRunning = true;
            if (that.workstationInfoLoaded !== true) return; // not ready
            if (that.currentOrder == undefined) return; // nothing to process
            var progress = (((model.speed * model.interval) / 1000) * 100) / that.workstationInfo.len;
            $scope.currentOrderProgress += progress;
        });

        TerminalHubProxy.start().done(function () {
            TerminalHubProxy.invoke('GetWorkstationInfo', function() {
                console.log('get workstation info done');
            });
            TerminalHubProxy.invoke('GetCurrentOrderInfo', function() {
                console.log('get current order info done');
            });
        });

        /******************************************************
         * Private methods
         ********************************************************/
        function populateTasks(tasks) {
            tasks = _.sortBy(tasks, 'order');
            for (var i = 0; i < tasks.length; i++) {
                tasks[i].hasImage = tasks[i].img != null;
                tasks[i].hasEquipment = tasks[i].eq != null;
                tasks[i].hasComment = tasks[i].c != null;
                tasks[i].selected = false;
            }
            $scope.tasks = tasks;
        }

        function toggleConveyanceBeltRunStatus() {
            if ($scope.conveyanceBeltRunning === true) {
                TerminalHubProxy.invoke('StopConveyanceBelt', function () {
                    console.log('Stop conveyance belt triggered');
                });
            } else {
                TerminalHubProxy.invoke('StartConveyanceBelt', function () {
                    console.log('Start conveyance belt triggered');
                });
            }
        }

        function selectUp() {
            console.log("select up " + that.selectedIndex);
            if ($scope.tasks.length < 1) return;
            if (that.selectedIndex != undefined) {
                $scope.tasks[that.selectedIndex].selected = false;
                that.selectedIndex--;
                if (that.selectedIndex < 0) that.selectedIndex = $scope.tasks.length - 1;
            } else {
                that.selectedIndex = 0;
            }
            $scope.selectedItem = $scope.tasks[that.selectedIndex];
            $scope.tasks[that.selectedIndex].selected = true;
        }

        function selectDown() {
            console.log("selet down " + that.selectedIndex);
            if ($scope.tasks.length < 1) return;
            if (that.selectedIndex != undefined) {
                $scope.tasks[that.selectedIndex].selected = false;
                that.selectedIndex++;
                if (that.selectedIndex >= $scope.tasks.length) that.selectedIndex = 0;
            } else {
                that.selectedIndex = 0;
            }
            $scope.selectedItem = $scope.tasks[that.selectedIndex];
            $scope.tasks[that.selectedIndex].selected = true;
        }

        function showSelectedImage(selectedItem) {
            console.log("show image");
            if (selectedItem == undefined) return;
            $mdDialog.show({
                controller: ShowImageDialogController,
                templateUrl: 'templates/dialogs/showimage.tpl.html',
                parent: angular.element(document.body),
                locals: {
                    imageBaseUrl: imageProviderBaseUrl,
                    selectedItem: selectedItem
                }
            });
        }

        function writeCommentToSelected(selectedItem) {
            console.log("write comment");
            if (selectedItem == undefined) return;
            $mdDialog.show({
                controller: ShowCommentDialogController,
                templateUrl: 'templates/dialogs/editcomment.tpl.html',
                parent: angular.element(document.body),
                locals: {
                    selectedItem: selectedItem,
                    orderId :$scope.currentOrder,
                    hubProxy: TerminalHubProxy
                }
            });
        }
    }

    function ShowImageDialogController($scope, $mdDialog, imageBaseUrl, selectedItem) {
        $scope.selectedItem = selectedItem;
        $scope.imageBaseUrl = imageBaseUrl;
        $scope.close = function () {
            $mdDialog.hide();
        }
    }

    function ShowCommentDialogController($scope, $mdDialog, selectedItem, orderId, hubProxy) {
        $scope.selectedItem = selectedItem;
        $scope.myComment = selectedItem.c;
        $scope.originalComment = selectedItem.c;
        $scope.orderId = orderId;
        $scope.close = function () {
            $mdDialog.hide();
        }
        $scope.save = function () {
            $mdDialog.hide().then(function () {
                if ($scope.originalComment !== $scope.myComment) {
                    console.log('Comment modified. Updating entry');
                    hubProxy.invoke('ChangeTaskComment', function () {
                        $scope.selectedItem.c = $scope.myComment;
                        $scope.selectedItem.hasComment = true;
                        console.log('comment saved');
                    }, $scope.orderId, $scope.selectedItem.id, $scope.myComment);
                }
            });
        }
    }
})(jQuery, _);