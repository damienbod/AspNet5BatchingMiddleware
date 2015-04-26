(function () {
	'use strict';

	var module = angular.module('myapp');

	module.controller('homeController',
		[
			'$scope',
			'$http',
			'$log',
			homeController
		]
	);

	function homeController($scope, $http, $log) {
		console.log('home called');
	}

	//function DetailsController($scope, $log, fastestAnimal) {
	//	$log.info("DetailsController called");
	//	$scope.message = "Animal Details";

	//	$scope.animal = fastestAnimal;

	//}

})();

