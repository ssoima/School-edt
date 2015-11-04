(function () {
  'use strict';

  angular
    .module('nextlap.authentication', [
      'nextlap.authentication.controllers',
      'nextlap.authentication.services'
    ]);

  angular
    .module('nextlap.authentication.controllers', []);

  angular
    .module('nextlap.authentication.services', ['ngCookies']);
})();