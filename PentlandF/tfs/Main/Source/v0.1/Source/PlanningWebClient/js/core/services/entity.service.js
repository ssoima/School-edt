(function() {
    'use strict';

    angular.module('nextlap.core')
        .service('EntityBase', EntityBaseService);

    EntityBaseService.$inject = ['$http', '$rootScope'];

    function EntityBaseService($http, $rootScope) {
        var url = $rootScope.apiBaseUrl;
        var ep = '';

        return {
            setEndpoint: setEndPoint,
            create: create,
            update: update,
            remove: remove,
            get: get,
            getAll: getAll
        }

        function setEndPoint(endpoint) {
            ep = endpoint;
        }

        function getAll() {
            if (ep == '') throw 'You must set the endpoint first.';
            return $http({
                url: url + '/' + ep,
                method: 'GET'
            });
        }

        function get(id) {
            if (ep == '') throw 'You must set the endpoint first.';
            return $http({
                url: url + '/' + ep + '/' + id,
                method: 'GET'
            });
        }

        function create(model) {
            if (ep == '') throw 'You must set the endpoint first.';
            return $http({
                url: url + '/' + ep + '/create',
                method: 'POST',
                data: model
            });
        }

        function update(model) {
            if (ep == '') throw 'You must set the endpoint first.';
            return $http({
                url: url + '/' + ep + '/update',
                method: 'PUT',
                data: model
            });
        }

        function remove(id) {
            if (ep == '') throw 'You must set the endpoint first.';
            return $http({
                url: url + '/' + ep + '/delete/' + id,
                method: 'DELETE'
            });
        }
    }
})();