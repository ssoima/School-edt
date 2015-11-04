/**
* FactoryController
* @namespace nextlap.factory.controllers
*/
(function () {
  'use strict';

  angular
    .module('nextlap.factory.controllers')
    .controller('FactoryController', FactoryController);

  FactoryController.$inject = ['$location', '$scope', 'Factory', 'debug'];

  /**
  * @namespace FactoryController
  */
  function FactoryController($location, $scope, Factory, debug) {
      var vm = this;

      function getCluster(factory_id) {
          Factory.getCluster(factory_id).then(function success(data) {
              vm.name = data.data.Name;
              vm.description = data.data.Description;
              vm.id = data.data.Id;
              vm.editFactory = editFactory;
              vm.factories = data.data;

          }, function error(data) {
              debug && console.log("Error loading factories", data);
          }).finally(function () {
          });
      }
      getCluster(null);
      function editFactory() {
          $location.path('/assembly-lines');
      }

      debug && $scope.$on('$destroy', function() {
            console.debug('Destroying FactoryController');
        });
      debug && console.debug('Creating FactoryController');
  }

})();