/**
 * Created by dmorina on 21/05/15.
 */
/**
* ClusterController
* @namespace nextlap.factory.controllers
*/
(function () {
    'use strict';

    angular
      .module('nextlap.factory.controllers')
      .controller('ClusterController', ClusterController);

    ClusterController.$inject = ['$location', '$scope', 'Cluster', '$animate', '$window','$routeParams'];

    /**
    * @namespace ClusterController
    */
    function ClusterController($location, $scope, Cluster, $animate, $window,$routeParams) {
        var self = this;
        self.params = $routeParams;
        self.addCluster = addCluster;
        function getClusters(parent) {
            Cluster.getClusters(parent).then(function success(data, status) {
                vm.clusters = data.data;
            }, function error(data, status) {
                console.log("errors" + status);
            }).finally(function () {
            });
        }
        function addCluster(parent, previous, next, name, description, type) {
            Cluster.addCluster(parent, previous, next, name, description, type).then(function success(data, status) {
                //vm.clusters = data.data;
            }, function error(data, status) {
                console.log("errors" + status);
            }).finally(function () {
            });
        }

    }
})();