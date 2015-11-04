/**
* SectionController
* @namespace nextlap.factory.controllers
*/
(function () {
    'use strict';

    angular
      .module('nextlap.factory.controllers')
      .controller('SectionController', SectionController);

    SectionController.$inject = ['$location', '$scope', 'Section', '$animate', '$window','$routeParams', 'debug'];

    /**
    * @namespace SectionController
    */
    function SectionController($location, $scope, Section, $animate, $window, $routeParams, debug) {
        var self = this;
        self.params = $routeParams;
        function getSections(factory_id) {
            Section.getSections(factory_id).then(function success(data) {
                self.sections = data.data;
            }, function error(data, status) {
                console.log("errors" + status);
            }).finally(function () {
            });
        }
        getSections(self.params.lineId);
        $scope.ctrl.setStepId();

        debug && $scope.$on('$destroy', function() {
            console.debug('Destroying AssemblyLineController');
        });
        debug &&console.debug('Creating AssemblyLineController');
    }
})();