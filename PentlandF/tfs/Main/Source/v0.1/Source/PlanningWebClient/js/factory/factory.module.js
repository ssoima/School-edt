(function () {
    'use strict';

    angular
      .module('nextlap.factory', [
        'nextlap.factory.controllers',
        'nextlap.factory.services'
      ]);

    angular
      .module('nextlap.factory.controllers', ['ngRoute']);

    angular
      .module('nextlap.factory.services', ['ngCookies']);
})();