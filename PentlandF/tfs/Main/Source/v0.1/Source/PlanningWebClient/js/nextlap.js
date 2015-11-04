/**
 * @desc Main module declaration and instantiation
 */

angular
    .module('nextlap', [
        'ngAnimate',
        'ngSanitize',
        'ngMaterial',
        'nextlap.config',
        'nextlap.routes',
        'nextlap.core',
        'nextlap.authentication',
        'nextlap.home',
        'nextlap.factory',
        'nextlap.advanced',
        'nextlap.assignment-source',
        'ngDragDrop',
        'nextlap.data-table',
        'ngFileUpload'
    ]);

angular
    .module('nextlap')
    .run(run)
    .constant("debug", false);

run.$inject = ['$http', '$rootScope', '$location'];

/**
 * @name run
 * @desc Update xsrf $http headers to align with Django's defaults, secondly it attaches an eventHandler
 * on $routeChangeStart to handle the `mode` by setting it to the value of the property specified for this route
 */
function run($http, $rootScope, $location) {
    $http.defaults.xsrfHeaderName = 'X-CSRFToken';
    $http.defaults.xsrfCookieName = 'csrftoken';
    $rootScope.$on('$routeChangeStart', function (event, next) {
      if (next.hasOwnProperty('$$route') && next.$$route.hasOwnProperty('mode')) {
          $location.search('mode', next.$$route.mode);
      }
    });
}