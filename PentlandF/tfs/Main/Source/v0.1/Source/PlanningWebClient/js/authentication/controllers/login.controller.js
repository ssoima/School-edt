/**
* LoginController
* @namespace nextlap.authentication.controllers
*/
(function () {
  'use strict';

  angular
    .module('nextlap.authentication.controllers')
    .controller('LoginController', LoginController);

  LoginController.$inject = ['$window', '$location', '$scope', 'Authentication'];

  /**
  * @namespace LoginController
  */
  function LoginController($window, $location, $scope, Authentication) {
    var vm = this;

    vm.login = login;

    activate();

    /**
    * @name activate
    * @desc Actions to be performed when this controller is instantiated
    * @memberOf nextlap.authentication.controllers.LoginController
    */
    function activate() {
      // If the user is authenticated, they should not be here.
      if (Authentication.isAuthenticated()) {
        $location.url('/');
      }
    }

    /**
    * @name login
    * @desc Log the user in
    * @memberOf nextlap.authentication.controllers.LoginController
    */
    function login() {
      //cfpLoadingBar.start();
      
      Authentication.login(vm.email, vm.password).then(function success(data, status) {
      
        Authentication.setAuthenticatedAccount(data.data);
        //$window.location = '/home'
            Authentication.getOauth2Token(vm.email, vm.password,
                "password", data.data.client_id, data.data.client_secret).then(function success(data, status) {
                Authentication.setOauth2Token(data.data);
                $window.location = '/home'
              }, function error(data, status) {



          });
      }, function error(data, status) {
      
      }).finally(function () {
        //cfpLoadingBar.complete();
      });
    }
  }
})();