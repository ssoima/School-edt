/**
 * @desc Few controllers which are shared by the other controllers, they act as utils, can be moved to services
 * @namespace nextlap.home.controllers
 */
(function () {
    'use strict';

    angular
        .module('nextlap.home.controllers')
        .controller('HomeController', HomeController);
    angular
        .module('nextlap.home.controllers')
        .controller('SearchController', SearchController);
    angular
        .module('nextlap.home.controllers')
        .controller('BrowseController', BrowseController);
    angular
        .module('nextlap.home.controllers')
        .controller('DetailController', DetailController);
    angular
        .module('nextlap.home.controllers')
        .controller('NotifyController', NotifyController);

    HomeController.$inject = ['$location', '$log', '$scope', '$rootScope', '$mdSidenav', '$mdUtil', '$routeParams', 'Factory', '$filter', 'AssemblyLine', 'Section', 'Station'];
    SearchController.$inject = ['$location', '$scope', '$log', '$q', '$timeout', 'Authentication', '$mdSidenav', '$mdUtil'];
    BrowseController.$inject = ['$location', '$scope', 'debug'];
    DetailController.$inject = ['$location', '$scope', '$log', '$q', '$timeout', 'Authentication', '$mdSidenav', '$mdUtil'];
    NotifyController.$inject = ['$scope', '$mdToast', '$animate'];

    /**
     * @name HomeController
     * @desc defines few almost `root-scope` variables which are used across the other controllers & services
     */
    function HomeController($location, $log, $scope, $rootScope, $mdSidenav, $mdUtil, $routeParams, Factory, $filter, AssemblyLine, Section, Station) {
        var self = this;
        self.location = $location;
        self.showTabs = showTabs;
        self.showLeftSideNav = showLeftSideNav;
        self.navigate = navigate;
        self.navigateTo = navigateTo;
        self.getMode = getMode;
        self.stepId = 1;
        self.advanced_breadcrumb = [];
        $rootScope.routeParams = $routeParams;
        //$rootScope.apiBaseUrl = 'http://ip1webapi.azurewebsites.net';
        $rootScope.apiBaseUrl = 'http://localhost/IP1PlanningWebAPI';
        $rootScope.backend_type = '.NET';
        self.switchStep = switchStep;
        self.stationTab = 'sequence';
        self.setStationTab = setStationTab;
        self.getStationTab = getStationTab;
        self.equipmentFormType = null;
        setStepId();
        self.showWorkSteps = showWorkSteps;
        self.setStepId = setStepId;
        /*
        * @desc rebuilds the breadcrumb on $routeChangeSuccess, note they dont query each time only when it's necessary. However it would be better if they
        * were moved to a more persistent medium
        * */
        $scope.$on('$routeChangeSuccess', function () {
            self.advanced_breadcrumb = [];
            self.advanced_breadcrumb.push({level: 0, display: 'Browse', href: '/browse', exists: true});
            Factory.factories().then(function success(data) {
                var factory = $filter('filter')(data, {id: $rootScope.routeParams.factoryId});
                if (factory.length) {
                    self.advanced_breadcrumb.push({
                        level: 1,
                        display: factory[0].name,
                        href: '/browse/factory/' + $rootScope.routeParams.factoryId,
                        exists: $rootScope.routeParams.factoryId
                    });
                }
            });
            if ($rootScope.routeParams.lineId) {
                AssemblyLine.assemblyLines($rootScope.routeParams.factoryId).then(function success(data) {
                    var line = $filter('filter')(data, {id: $rootScope.routeParams.lineId});
                    if (line.length) {
                        self.advanced_breadcrumb.push({
                            level: 2,
                            display: line[0].name,
                            href: '/browse/factory/' + $rootScope.routeParams.factoryId + '/line/' + $rootScope.routeParams.lineId,
                            exists: $rootScope.routeParams.lineId
                        });
                    }

                });
            }
            if ($rootScope.routeParams.sectionId) {
                Section.sections($rootScope.routeParams.lineId).then(function success(data) {
                    var section = $filter('filter')(data, {id: $rootScope.routeParams.sectionId});
                    if (section.length) {
                        self.advanced_breadcrumb.push({
                            level: 3,
                            display: section[0].name,
                            href: '/browse/factory/' + $rootScope.routeParams.factoryId
                            + '/line/' + $rootScope.routeParams.lineId + '/section/' + $rootScope.routeParams.sectionId,
                            exists: $rootScope.routeParams.sectionId
                        });
                    }

                });
            }
            if ($rootScope.routeParams.stationId) {
                Station.stations($rootScope.routeParams.sectionId).then(function success(data) {
                    var station = $filter('filter')(data.data, {id: $rootScope.routeParams.stationId});
                    if (station.length) {
                        self.advanced_breadcrumb.push({
                            level: 4,
                            display: station[0].name,
                            href: '/browse/factory/' + $rootScope.routeParams.factoryId
                            + '/line/' + $rootScope.routeParams.lineId + '/section/' + $rootScope.routeParams.sectionId +
                            '/station/' + $rootScope.routeParams.stationId,
                            exists: $rootScope.routeParams.stationId
                        });
                    }

                });
            }

            self.advanced_breadcrumb = $filter('orderBy')(self.advanced_breadcrumb, 'level');
        });
        /*
        * @desc switches tabs for the `Advanced` view of the station
        * */
        function setStationTab(tab){
            self.stationTab = tab;
        }
        function getStationTab(){
            return self.stationTab;
        }
        function showTabs() {
            return (self.location.path() === '/' || self.location.path() === '/browse');
        }

        function showLeftSideNav() {
            if (self.stationTab === 'overview' || (self.equipmentFormType && self.equipmentFormType.match(/shelf/gi)))
                return false;
            var showLeftNav = (self.location.path().match(/(section|workstation|equipments)/gi) && self.getMode().match(/(wizard|advanced)/gi)) === null;
            return !showLeftNav;
        }

        function navigate(path) {
            var url = (path === '/browse') ? '/browse' : '/';
            $location.url(url);
        }

        self.toggleLeft = buildToggler('left');
        self.toggleRight = buildToggler('right');

        function getMode() {
            return $routeParams.mode ? $routeParams.mode : 'wizard';
        }

        $scope.toggleNavbar = function () {
            $scope.showNavbar = !$scope.showNavbar;
        };


        function buildToggler(navID) {
            var debounceFn = $mdUtil.debounce(function () {
                $mdSidenav(navID)
                    .toggle()
                    .then(function () {
                        $log.debug("toggle " + navID + " is done");
                    });
            }, 300);
            return debounceFn;
        }

        function navigateTo(path, append) {
            //var mode = getMode();
            if (append) {
                var currentPath = $location.path();
                $location.path(currentPath + '/' + path);
            }
            else {
                $location.path(path);
            }

            //$location.search('mode', mode);

        }

        function setStepId() {
            var path = $location.path();
            if (path.match(/workstations/gi)) {
                self.stepId = 2;
            }
            else if (path.match(/equipments/gi)) {
                self.stepId = 3;
            }
            else {
                self.stepId = 1;
            }
            $rootScope.$emit('stepChanged', {message: "OK", step: self.stepId});
        }

        function switchStep(stepName) {
            var path = "";
            var mode = getMode();
            var base = '/browse/factory/' + $routeParams.factoryId +
                '/line/' + $routeParams.lineId + '/section/' + $routeParams.sectionId;
            if (stepName !== 'stations') {
                path = "/" + stepName;
            }
            $location.path(base + path);
            $location.search('mode', mode);
            setStepId();
        }

        function showWorkSteps() {
            var path = $location.path();
            return path.match(/section/gi) && self.getMode() === 'wizard';
        }
    }


    /**
     * @namespace SearchController
     * @desc Handles the Main Search page, at the moment it's purely static for demo purposes, later to be linked to the backend to perform Full-Text-Search
     */
    function SearchController($location, $scope, $log, $q, $timeout, Authentication, $mdSidenav) {
        var self = this;
        self.simulateQuery = true;
        self.isDisabled = false;
        // list of `state` value/display objects
        self.results = loadAll();
        self.querySearch = querySearch;
        self.selectedItemChange = selectedItemChange;
        self.searchTextChange = searchTextChange;
        function querySearch(query) {
            var results = query ? self.results.filter(createFilterFor(query)) : self.results,
                deferred;

            if (self.simulateQuery) {
                deferred = $q.defer();
                $timeout(function () {
                    deferred.resolve(results);
                }, Math.random() * 1000, false);
                return deferred.promise;
            } else {
                return results;
            }
        }

        function searchTextChange(text) { // jshint ignore:line
        }

        function selectedItemChange(item) { // jshint ignore:line
        }

        function loadAll() {
            var allResults = 'Part 8711, Part 8710, Part 8713, Part 8714, Part x4, Section 8719, Worker 9822, Device #13, Station 42';
            return allResults.split(/, +/g).map(function (result) {
                return {
                    value: result.toLowerCase(),
                    display: result
                };
            });
        }

        function createFilterFor(query) {
            var lowercaseQuery = angular.lowercase(query);
            return function filterFn(result) {
                var re = new RegExp(lowercaseQuery, 'gi');
                return (result.value.match(re));
            };
        }

        self.close = function () {
            $mdSidenav('left').close()
                .then(function () {
                });
        };
    }

    function BrowseController($location, $scope, debug) {
        var self = this;
        self.location = $location;
        self.showTabs = showTabs;
        self.navigate = navigate;
        function showTabs() {
            return (self.location.path() === '/' || self.location.path() === '/browse');
        }

        function navigate(path) {
            var url = (path === '/browse') ? '/browse' : '/';
            $location.url(url);
        }

        debug && $scope.$on('$destroy', function() {
            console.debug('Destroying BrowseController');
        });
        debug && console.debug('Creating BrowseController');
    }

    function DetailController($location, $scope, $log, $q, $timeout, Authentication, $mdSidenav) {
        var self = this;
        self.close = function () {
            $mdSidenav('right').close()
                .then(function () {
                });
        };
    }
    /**
     * @name NotifyController
     * @desc Displays the `toast` notifications, used in many other controllers
    * */
    function NotifyController($scope, $mdToast) {
        var self = this;
        self.notifyChange = notifyChange;
        function notifyChange(message) {
            console.log('showing message', message);
            $mdToast.show(
                $mdToast.simple()
                    .content(message)
                    .position('bottom right')
                    .hideDelay(5000)
            );
        }
    }
})();