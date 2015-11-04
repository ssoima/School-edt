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
      .module('nextlap.data-table.controllers')
      .controller('DataTableController', DataTableController);

    DataTableController.$inject = ['$location', '$scope','$log', '$q','$timeout', '$mdSidenav', '$mdUtil', '$rootScope', '$filter', '$routeParams'];
    /**
    * @namespace DataTableController
    */
      function DataTableController($location, $scope,  $log,$q ,$timeout, $mdSidenav, $mdUtil, $rootScope, $filter, $routeParams) {
        var self = this;
        self.isDisabled    = false;
        function activate(){
        }
        self.activate();



      }
})();