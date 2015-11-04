(function () {
    'use strict';

    angular.module('workstationTerminal', [
            'ngRoute',
            'ngAnimate',
            'ngAria',
            'ngMaterial',
            'cfp.hotkeys'
    ])
        //.value('signalrUrl', 'http://ip1wsapi.azurewebsites.net')
        //.value('workstationServiceUrl', 'http://ip1wsapi.azurewebsites.net')
        //.value('imageProviderBaseUrl', 'http://ip1webapi.azurewebsites.net')
        .value('signalrUrl', 'http://localhost/IP1WorkstationService')
        .value('workstationServiceUrl', 'http://localhost/IP1WorkstationService')
        .value('imageProviderBaseUrl', 'http://localhost/IP1PlanningWebAPI')
        .config(configRoute)
        .config(configLocation);

    configRoute.$inject = ['$routeProvider'];

    function configRoute($routeProvider) {
        $routeProvider.when('/:wsid', {
            controller: 'CurrentTasksController',
            templateUrl: 'templates/currenttasks.html',
            hotkeys: [
                ['up', '', 'selectUp()'],
                ['down', '', 'selectDown()'],
                ['f2', 'Show image', 'showSelectedImage(selectedItem)'],
                ['f4', 'Write comment', 'writeCommentToSelected(selectedItem)']
            ]
        });
    }

    configLocation.$inject = ['$locationProvider'];

    function configLocation($locationProvider) {
        $locationProvider.html5Mode(true);
    }
})();