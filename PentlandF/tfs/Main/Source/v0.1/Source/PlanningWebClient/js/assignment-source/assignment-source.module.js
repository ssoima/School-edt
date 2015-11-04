(function () {
    'use strict';

    angular
      .module('nextlap.assignment-source', [
        'nextlap.assignment-source.controllers',
        'nextlap.assignment-source.services'
      ]);

    angular
      .module('nextlap.assignment-source.controllers', []);

    angular
      .module('nextlap.assignment-source.services', ['ngCookies']);
})();