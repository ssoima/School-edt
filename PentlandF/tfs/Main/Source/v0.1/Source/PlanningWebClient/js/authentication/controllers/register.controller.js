/**
* Register controller
* @namespace nextlap.authentication.controllers
*/
(function () {
  'use strict';

  angular
    .module('nextlap.authentication.controllers')
    .controller('RegisterController', ['$location', '$scope', 'Authentication',
      function RegisterController($location, $scope, Authentication) {

        activate();
        /**
         * @name activate
         * @desc Actions to be performed when this controller is instantiated
         * @memberOf nextlap.authentication.controllers.RegisterController
         */
        function activate() {
          // If the user is authenticated, they should not be here.
          if (Authentication.isAuthenticated()) {
            $location.url('/home');
          }
        }
        var vm = this;

        vm.register = register;

        /**
        * @name register
        * @desc Register a new user
        * @memberOf nextlap.authentication.controllers.RegisterController
        */
        function register() {
          //cfpLoadingBar.start();
          Authentication.register(vm.email, vm.firstname, vm.lastname,
            vm.password1, vm.password2).then(function () {
              
              $location.url('/login');
            }, function (data, status) {

            }).finally(function () {
              //cfpLoadingBar.complete();
            });
        }
    }]);
})();