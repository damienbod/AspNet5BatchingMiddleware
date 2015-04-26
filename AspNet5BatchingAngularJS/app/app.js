(function () {
	'use strict';

	var app = angular.module('myapp', [
		'ui.router',
		'jcs.angular-http-batch'
	]);

	app.config(['httpBatchConfigProvider', function (httpBatchConfigProvider) {
		httpBatchConfigProvider.setAllowedBatchEndpoint(
			// root endpoint url
			'http://localhost:13605/api',

			// endpoint batch address
			'http://localhost:13605/api/batch',

			// optional configuration parameters
			{
				maxBatchedRequestPerCall: 20
			});
		}
	]);

	app.config(["$stateProvider", "$urlRouterProvider",
	function ($stateProvider, $urlRouterProvider) {
		$urlRouterProvider.otherwise("/home");
		$stateProvider.state("home", {
			url: "/home", templateUrl: "/templates/home.html", controller: "homeController",
			resolve: {
				TestClassOneService: "TestClassOneService",

				testClassOne: ["TestClassOneService", function (TestClassOneService) {
					return TestClassOneService.getAll();
				}],
				TestClassTwoService: "TestClassTwoService",

				testClassTwo: ["TestClassTwoService", function (TestClassTwoService) {
					return TestClassTwoService.getAll();
				}]
			}
		});
	}
	]);

})();