(function () {
    'use strict';

    angular.module('executionEngine', [
            'ngAnimate',
            'ngAria',
            'ngMaterial',
            'angularMoment'
        ])
        //.value('baseApi', 'http://ip1exeapi.azurewebsites.net');
        .value('baseApi', 'http://localhost/IP1ExecutionEngineWebAPI');
})();