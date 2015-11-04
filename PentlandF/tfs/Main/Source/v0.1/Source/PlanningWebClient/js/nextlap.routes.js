(function () {
    'use strict';

    angular
        .module('nextlap.routes', ['ngRoute'])
        .config(config);

    config.$inject = ['$routeProvider'];

    /**
     * @name config
     * @desc Define valid application routes
     */
    function config($routeProvider) {
        $routeProvider.when('/search', {
            templateUrl: '/static/templates/search.html',
            controller: 'SearchController',
            mode: 'wizard'
        })
            .when('/', {
                controller: 'BrowseController',
                controllerAs: 'vm',
                templateUrl: '/static/templates/browse.html',
                mode: 'wizard'
            }).when('/browse/factory/:factoryId', {
                controller: 'AssemblyLineController',
                controllerAs: 'assembly',
                templateUrl: '/static/templates/assembly-line.html',
                mode: 'wizard'
            }).when('/browse/factory/:factoryId/line/:lineId', {
                controller: 'SectionController',
                controllerAs: 'section',
                templateUrl: '/static/templates/section.html',
                mode: 'wizard'
            }).when('/browse/factory/:factoryId/line/:lineId/section/:sectionId/station/:stationId/sequence', {
                controller: 'AdvancedController',
                controllerAs: 'advanced',
                templateUrl: '/static/templates/advanced/advanced-base.html',
                mode: 'advanced'
            }).when('/browse/factory/:factoryId/line/:lineId/section/:sectionId', {
                controller: 'StationController',
                controllerAs: 'station',
                templateUrl: '/static/templates/station.html',
                mode: 'wizard'
            }).when('/browse/factory/:factoryId/line/:lineId/section/:sectionId/workstations/:stationId?', {
                controller: 'WorkStationController',
                controllerAs: 'workstation',
                templateUrl: '/static/templates/workstation.html',
                mode: 'wizard'
            }).when('/browse/factory/:factoryId/line/:lineId/section/:sectionId/equipments/:stationId?', {
                controller: 'EquipmentController',
                controllerAs: 'equipment',
                templateUrl: '/static/templates/equipment.html',
                mode: 'wizard'
            }).otherwise({redirectTo: '/'});
    }
})();