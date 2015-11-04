(function () {
    'use strict';

    angular
      .module('nextlap.advanced', [
        'nextlap.advanced.controllers',
        'nextlap.advanced.services'
      ]);

    angular
        .module('nextlap.advanced.controllers', [
            'ngRoute',
            'nextlap.core',
            'nextlap.factory'
        ]);

    angular
      .module('nextlap.advanced.services', []);
})();