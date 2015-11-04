/**
 * Created by dmorina on 28/06/15.
 */

/**
 * EquipmentController
 * @namespace nextlap.factory.controllers
 */
(function () {
    'use strict';


    angular
        .module('nextlap.factory.controllers')
        .controller('EquipmentController', EquipmentController);

    EquipmentController.$inject = ['$location', '$scope', 'Equipment', '$animate', '$window', '$routeParams', '$filter', 'DragAndDropAssignment',
        '$rootScope', 'StationTask'];

    /**
     * @namespace EquipmentController
     */
    function EquipmentController($location, $scope, Equipment, $animate, $window, $routeParams, $filter, DragAndDropAssignment,
                                 $rootScope, StationTask) {
        var self = this;
        self.params = $routeParams;
        self.equipments = [];
        self.getImage = getImage;
        self.getEquipments = getEquipments;
        self.name = null;
        self.droppableContainer = [];
        $scope.onOver = onOver;
        $scope.onDropEquipment = onDropEquipment;
        getEquipments(self.params.stationId);

        function onOver(event, ui) {

        }
        function onDropEquipment(event, ui) {
            var item = {
                data_id: $(ui.draggable).attr('data-id'),
                data_index: $(ui.draggable).attr('data-index'),
                data_type: $(ui.draggable).attr('data-type'),
                data_parent_id: $(ui.draggable).attr('data-parent-id'),
                data_parent_parent_id: $(ui.draggable).attr('data-parent-parent-id'),
                data_target: 'equipment'
            };
            var target = event.target.id;
            StationTask.assignEquipmentType(item.data_id, self.equipments[target].id, true).then(function success(data, status) {
                $rootScope.$emit('itemDropped', { message: "OK", item: item, target: self.equipments[target] });
            }, function error(data, status) {
                $scope.notifyCtrl.notifyChange('Equipment assignment failed: '+JSON.stringify(data.data));
            }).finally(function () {

            });
        }
        function getEquipments(parent) {
                Equipment.getTypes(parent).then(function success(data, status) {
                    self.equipments = data.data;
                }, function error(data, status) {
                    $scope.notifyCtrl.notifyChange('Failed to load equipments: '+JSON.stringify(data.data));
                }).finally(function () {
                    self.resetCreateForm();
                });
        }
        function getImage(name) {
            if(!name) return '';
            if(name.match(/scanner/gi)) return '/static/images/equipment/scanner.jpg';
            else if (name.match(/shelf/gi) && name.match(/light/gi)) return '/static/images/equipment/shelf-pbl.jpg';
            else if (name.match(/shelf/gi) && name.match(/voice/gi)) return '/static/images/equipment/shelf-pbv.png';
            else if (name.match(/shelf/gi) && name.match(/regular/gi)) return '/static/images/equipment/shelf-regular.jpg';
            else if (name.match(/screwdriver/gi) && name.match(/ec/gi) && (name.match(/akku/gi)||name.match(/accu/gi))) return '/static/images/equipment/screwdriver-ec-akku.jpg';
            else if (name.match(/screwdriver/gi) && name.match(/ec/gi)) return '/static/images/equipment/screwdriver-ec.jpg';
            else if (name.match(/screwdriver/gi)) return '/static/images/equipment/screwdriver.jpg';
            else if (name.match(/glass/gi) ) return '/static/images/equipment/ar-glass.png';
            return '/static/images/material-design-icons/action/svg/production/ic_build_48px.svg';
        }
    }
})();