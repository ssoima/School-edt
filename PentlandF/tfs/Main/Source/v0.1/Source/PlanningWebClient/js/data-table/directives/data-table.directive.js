/**
 * Created by dmorina on 21/05/15.
 */
/**
* DataTableController
* @namespace nextlap.data-table.controllers
*/
(function () {
    'use strict';

    angular
      .module('nextlap.data-table.directives')
      .directive('mdDataTable', mdDataTableDirective);
    angular
      .module('nextlap.data-table.directives')
      .directive('mdDataTableCardHeader', mdDataTableCardHeaderDirective);
    angular
      .module('nextlap.data-table.directives')
      .directive('mdDataTableCardHeaderTitle', mdDataTableCardHeaderTitleDirective);
    angular
        .module('nextlap.data-table.directives')
        .directive('mdSortable', mdSortableDirective);
    function mdDataTableDirective(){
        return {
            restrict: 'E',
            transclude: false,
            scope: true

        };
    }
    function mdDataTableCardHeaderDirective(){
        return {
            restrict: 'E',
            transclude: false,
            //template: '1 item selected',
            scope: true,
            require: '^mdDataTable'
        };
    }

    function mdDataTableCardHeaderTitleDirective(){
        return {
            restrict: 'E',
            transclude: false,
            template: '1 item selected',
            scope: true,
            require: '^mdDataTableCardHeader'
        };
    }
    function mdSortableDirective(){
        return {
            restrict: 'EA',
            transclude: false,
            scope: false, //{
                //dragStart: '&dragStart',
                //dragStop: '&dragStop'
            //},
            link: function(scope, element, attrs){
                element.sortable({
                    start: scope.advanced.dragStart,
                    update: scope.advanced.dragStop,
                    axis: 'y',
                    placeholder: "row-placeholder"
                });
            }
        };
    }
})();