/**
* AssemblyLineController
* @namespace nextlap.factory.controllers
*/
(function () {
  'use strict';

  angular
    .module('nextlap.factory.controllers')
    .controller('AssemblyLineController', AssemblyLineController);

  AssemblyLineController.$inject = ['$location', '$scope', 'AssemblyLine','$animate','$window','$routeParams', 'debug'];

  /**
  * @namespace AssemblyLineController
  */
  function AssemblyLineController($location, $scope, AssemblyLine, $animate, $window, $routeParams, debug) {
      var vm = this;
      vm.params = $routeParams;
      function getAssemblyLines(factory_id) {
          AssemblyLine.getAssemblyLines(factory_id).then(function success(data) {
              vm.assembly_lines = data.data;
              vm.editAssemblyLine = editAssemblyLine;
              vm.back = back;
              //AssemblyLine.assemblyLines = data.data;
          }, function error(data, status) {
              console.log("errors" +status);
          }).finally(function () {
          });
      }
      getAssemblyLines(vm.params.factoryId);
      function editAssemblyLine() {
          $location.path('/section');
      }
      function back() {
          $window.history.back();
          //$animate.enter($("#factoryCard"));
          
      }

      debug && $scope.$on('$destroy', function() {
          console.debug('Destroying AssemblyLineController');
        });
      debug &&console.debug('Creating AssemblyLineController');
  }
})();