/* Controllers */
'use strict';

angular.module('maximuslife.controllers', [])

.controller('NavController', [ '$scope', '$location', '$ionicSideMenuDelegate', '$ionicNavBarDelegate', 'NavService',
	function ($scope, $location, $ionicSideMenuDelegate, $ionicNavBarDelegate, NavService) {

	$scope.initVariables = function() {
		$scope.menuItems = NavService.getMenuItems();
	};
	
	$scope.goTo = function(page) {
		$ionicSideMenuDelegate.toggleLeft();
		$location.url('/' + page);
	};
	
	$scope.getBackBtnTitle = function() {
		var backBtnTitle = NavService.getBackBtnTitle() || $ionicNavBarDelegate.getPreviousTitle();
		return backBtnTitle;
	};
	
	/* Initial Execution Code */
	$scope.initVariables();
}])
.controller('LoginController', ['$scope', '$stateParams', '$state', 'LoginService',
	function ($scope, $stateParams, $state, LoginService) {

	$scope.model = {
        // Profile
	    fullName: "",
	    mobileNumber: null,
	    employer: "",
	    birthDay: "",
	    gender: 'Select a gender',
        // Login crendential
	    email: "",
	    userName: "",
	    password: ""
	};

	$scope.userName = null;
	$scope.password = null;
	$scope.submitted = false;

	$scope.showProfileRegPage = function () {
	    $state.go('login.regprofile', {});
	};

	$scope.showCredentialRegPage = function (val) {
	    // Set the button submitted state.
	    $scope.submitted = true;
	    // Set the model to service while navigating
	    // from profile page.
	    $scope.setModel($scope.model);
	    if (val) {
	        $state.go('login.regcredential', {});
	    }
	};

	$scope.showSuccessPage = function (val) {
	    // Set the button submitted state.
	    $scope.submitted = true;

	    $scope.setModel($scope.model);
	    if (val) {
	        $state.go('login.success', {});
	    }
	};

	$scope.showStartPage = function (val) {
	    $state.go('login.start', {});
	};

	$scope.showLoginPage = function () {
	    $state.go('login.login', {});
	};

	$scope.showCategoryPage = function (val) {
	    $scope.submitted = true;
	    if (val) {
	        $state.go('categories', {});
	    }
	};

    // To set model to the service class
	$scope.setModel = function () {
	    LoginService.setModel($scope.model);
	}

    // To get the model from service class.
	$scope.initModel = function () {
	    var nmodel = LoginService.getModel();
	    if (null != nmodel) {
	        $scope.model = nmodel;
	    }
	    var str = "test";
	};

    /* Initial Execution Code */
	$scope.initModel();
	
}])
.controller('CategoryController', ['$scope', '$state', 'QuestService', function ($scope, $state, QuestService) {
	
	$scope.initVariables = function() {
		$scope.data = null;
	};
	
	$scope.getCategoriesData = function() {
		$scope.data = QuestService.getCategories();
	};

	$scope.showChildCategories = function(categoryId) {
	    $state.go('child-categories', { categoryId: categoryId });
	};
	
	/* Initial Execution Code */
	$scope.initVariables();
	$scope.getCategoriesData();
}])
.controller('ChildCategoryController', ['$scope', '$stateParams', '$state', 'QuestService', 'NavService',
	function ($scope, $stateParams, $state, QuestService, NavService) {
	
	$scope.currentSubCategoryIndex = 0; // To represent sub category index.

    // Method to get previous child cateogry index.
	$scope.showLeftSubCategory = function () {
	    $scope.currentSubCategoryIndex--;

	    if ($scope.currentSubCategoryIndex >= 0) {
	        // Get the Quests data
	        $scope.getQuestsData($scope.data.childCategories[$scope.currentSubCategoryIndex].id);
	    }
	};

	// Method to get next child cateogry index.
	$scope.showRightSubCategory = function () {
	    if ($scope.data.childCategories &&
            $scope.data.childCategories.length > 0) {
	        if ($scope.currentSubCategoryIndex < $scope.data.childCategories.length - 1) {
	            $scope.currentSubCategoryIndex++;
	        } else if (0 == $scope.currentSubCategoryIndex) {
	            $scope.currentSubCategoryIndex++;
	        }
	    }

	    if ($scope.currentSubCategoryIndex > 0) {
	        // Get the Quests data
	        $scope.getQuestsData($scope.data.childCategories[$scope.currentSubCategoryIndex].id);
	    }
	};
    
    // Method to get quests
    // @Param - {childCategoryId} - The child category id.
	$scope.getQuestsData = function (childCategoryId) {
	    try {
	        if (!childCategoryId) {
	            // We assume that the request is for initial time if child
                // category id is not provided.
	            childCategoryId = $scope.data.childCategories[0].id
	        }
	        $scope.questData = QuestService.getQuests(childCategoryId);
	    } catch (e) {
	        // Todo: Alert
	        console.log(e);
	    }
	}

	$scope.timeline = function (item) {
	    switch (item.timeframe) {
	        case "NinetyDays":
	            return "90";
	        case "OneYear":
	            return "One";
	        case "OwnDate":
	            return item.estimatedDate || "";
	        case "SixMonths":
	            return "Six";
	        case "UnderNinetyDays":
	            return "Under 90";
	        default:
	            return "";
	    }
	};

	$scope.timelineType = function (item) {
	    switch (item.timeframe) {
	        case "NinetyDays": return "Days";
	        case "OneYear": return "Year";
	        case "OwnDate": return "";
	        case "SixMonths": return "Months";
	        case "UnderNinetyDays": return "Days";
	        default: return "";
	    }
	};

    // Watching until child categories are available.
	$scope.$watch("data.childCategories", function (newValue, oldValue) {
	    if (newValue)
	        $scope.getQuestsData();
	});

    // Method to initialize controller variables.
	$scope.initVariables = function() {
		$scope.data = null;
	};
	
    // Method to get child categories.
	$scope.getChildCategoriesData = function() {
		$scope.data = QuestService.getCategory($stateParams.categoryId);
	};
	
    // Method to show the join quest page.
	$scope.showJoinQuest = function (questId) {
	    $state.go('join-quest', { questId: questId });
	};
	
	/* Initial Execution Code */
	$scope.initVariables();
	$scope.getChildCategoriesData();
	NavService.setBackBtnTitle('Setup');
}])

.controller('QuestController', ['$scope', '$ionicModal', '$filter', '$stateParams', '$state', '$ionicPopup', 'NavService', 'QuestService',
	function ($scope, $ionicModal, $filter, $stateParams, $state, $ionicPopup, NavService, QuestService) {

	    $scope.chartType = 'map';
	    
	    var filter = $filter('filter'), orderBy = $filter('orderBy');

		var fieldsOfType = function (fields, type) {
			return filter(fields || [], { type: type });
		};

		var activeFields = function (fields, type) {
			return filter(fieldsOfType(fields, type), { active: true });
		};

		var activeFieldNames = function (fields, type) {
			return activeFields(fields, type).map(function (f) { return f.name; });
		};

		var quest = QuestService.getQuest($stateParams.questId, function () {
		    $scope.model.selectedPurpose = activeFields(quest.fields, 'Purpose')[0];
		});

		$scope.data = quest;
		$scope.selectedApps = function () { return activeFieldNames(quest.fields, 'App').join(); };
		$scope.selectedAssets = function () { return activeFieldNames(quest.fields, 'Asset').join(); };
		$scope.selectedCheckinTypes = function () { return activeFieldNames(quest.fields, 'CheckinType').join(); };
		$scope.selectedDevices = function () { return activeFieldNames(quest.fields, 'Device').join(); };
		$scope.selectedMetrics = function () { return activeFieldNames(quest.fields, 'Metric').join(); };
		$scope.selectedPitfalls = function () { return activeFieldNames(quest.fields, 'Pitfall').join(); };
		$scope.selectedPurpose = function () { return activeFieldNames(quest.fields, 'Purpose')[0] || ''; }

		$scope.apps = function () { return fieldsOfType(quest.fields, "App"); }
		$scope.assets = function () { return fieldsOfType(quest.fields, "Asset"); }
		$scope.checkinTypes = function () { return fieldsOfType(quest.fields, "CheckinType"); }
		$scope.devices = function () { return fieldsOfType(quest.fields, "Device"); }
		$scope.metrics = function () { return fieldsOfType(quest.fields, "Metric"); }
		$scope.pitfalls = function () { return fieldsOfType(quest.fields, "Pitfall"); }
		$scope.purpose = function () { return fieldsOfType(quest.fields, "Purpose"); }

		// add any additional view model fields here, looks like we need an object
		// http://stackoverflow.com/questions/13632042/angularjs-two-way-data-binding-fails-if-element-has-ngmodel-and-a-directive-wit
		$scope.model = {
		    selectedPurpose: null,
            newField: "",
            checkin: {
                questId: $stateParams.questId,
                comment: null,
                checkpointWin: null,
                visibleInTimeline: null,
                CheckinType: 1
            }
		};

		$scope.$watch("model.selectedPurpose", function (newValue, oldValue) {
			if (newValue)
				newValue.active = true;
			if (oldValue)
				oldValue.active = false;
		});

		$scope.addField = function (type) {
			$ionicPopup.prompt({
				title: 'Add Field',
				template: 'Enter new ' + type,
				inputType: 'text',
				inputPlaceholder: ''
			})
			.then(function (name) {
				if (name == null)
					return;
				name = name.trim();
				if (!name.length)
					return;
				name = name.charAt(0).toUpperCase() + name.substr(1);
				var field = { type: type, name: name, active: true, deleted: false };
				quest.fields.push(field);
				if (type == "Purpose") {
					$scope.model.selectedPurpose = field;
				}
			});
		};

		$scope.$watch("data.timeframe", function (newValue, oldValue) {
			if (newValue != "OwnDate")
				quest.estimatedDate = "";
		});

		$scope.timeline = function () {
			switch (quest.timeframe) {
				case "NinetyDays":
					return "90";
				case "OneYear":
					return "One";
				case "OwnDate":
					return quest.estimatedDate || "";
				case "SixMonths":
					return "Six";
				case "UnderNinetyDays":
					return "Under 90";
				default:
					return "";
			}
		};

		$scope.timelineType = function () {
			switch (quest.timeframe) {
				case "NinetyDays": return "Days";
				case "OneYear": return "Year";
				case "OwnDate": return "";
				case "SixMonths": return "Months";
				case "UnderNinetyDays": return "Days";
				default: return "";
			}
		};

		$scope.checkinFrequencyDescription = function () {
			if (!quest.checkinsExpected || quest.checkinsExpected == 1)
				return quest.checkinFrequency;

			if (quest.checkinsExpected == 2)
				return "Twice " + (quest.checkinFrequency == "BiWeekly" ? "every two weeks" : quest.checkinFrequency);

			if (quest.checkinsExpected == 3)
				return "Thrice " + (quest.checkinFrequency == "BiWeekly" ? "every two weeks" : quest.checkinFrequency);

			return quest.checkinsExpected + " times "
						+ (quest.checkinFrequency == "BiWeekly" ? "every two weeks" : quest.checkinFrequency);
		};

		$scope.toggleGroup = function (group) {
			$scope.model.newField = "";
			if ($scope.isGroupShown(group))
				$scope.shownGroup = null;
			else
				$scope.shownGroup = group;
		};

		$scope.isGroupShown = function (group) {
			return $scope.shownGroup === group;
		};

		$scope.showMap = function () {
		    $scope.bCheckinVerify = false;
			$state.go('quest-map.chart-map', { questId: $stateParams.questId });
		};

		$scope.showCheckIn = function () {
		    $state.go('checkin', { questId: $stateParams.questId });
		};

		$scope.showCheckInVerify = function () {
		    $state.go('checkin-verify', { questId: $stateParams.questId });
		};

	    $scope.showQuestsDetail = function(questId) {
	        $state.go('quests-detail', { questId: $stateParams.questId });
	    };

	    $scope.showJoinQuest = function (questId) {
	        $state.go('join-quest', { questId: questId });
	    };

		$scope.showQuestDeclaration = function () {
		    $state.go('quest-declaration', { questId: $stateParams.questId });
		};

		$scope.range = function (n) {
		    // Locals.
		    var input = [];
		    for (var i = 0; i < n; i++) {
		        input.push(i);
		    }
		    return input;
		}

        // Method to format end date for timeline.
		$scope.formatedEndDate = function (str) {
		    return str.substring(0, str.lastIndexOf('/'));
		}

        // Method to show missed check-in message
		$scope.missedCheckins = function () {
		    var missed = quest.checkinsExpected - quest.checkinsActual;
		    return missed > 0 ? "You have " + missed + " missed check-ins" : "Congratulations no missed check-ins";
		};

        // Method for next check-in day message.
		$scope.nextCheckin = function () {
            // Locals
		    var msg = null;
		    var days = 0;
		    
		    switch (quest.checkinFrequency) {
		        case "Weekly":
		            days = 7 - quest.daysSinceLastCheckin;
		        break;
		        case "Monthly":
		            days = 30 - quest.daysSinceLastCheckin;
		        case "Daily":
		            days = daysSinceLastCheckin;
		        case "BiWeekly":
		            days = 15 - quest.daysSinceLastCheckin;
		        break;
		    }

		    return days;
		}

	    // To identify check-in verification required or not.
		$scope.needsVerification = false;
		$scope.bCheckinVerify = false;

		$scope.getSelectClass = function (item) {

		    if (item.isCurrent) {
		        $scope.needsVerification = item.needsVerification;
		    }
		    return item.isCurrent ? 'revhr-checkin-green' : 'revhr-checkin-white'
		}

        	// Method to save current check-in
		$scope.saveCheckin = function () {
		    $scope.saveCheckinResult = QuestService.saveCheckin($scope.model.checkin);
		    if ($scope.needVarification) {
		        $scope.getPhoto();
		    }
		};

	    	// Method to open camera for verify image
		$scope.getPhoto = function () {

		    try {

		        var onSuccess = function(imageData) {
		    	    // Attn: Some logic need to be updated here.
		            // image.src = "data:image/jpeg;base64," + imageData;
			}

		        var onFail = function (message) {
		            alert('Failed to open camera: ' + message);
		        }

		        navigator.camera.getPicture(onSuccess, onFail, {
		            quality: 50,
		            destinationType: Camera.DestinationType.DATA_URL,
		            sourceType : Camera.PictureSourceType.CAMERA,
		            allowEdit : true,
		            encodingType: Camera.EncodingType.JPEG,
		            targetWidth: 100,
		            targetHeight: 100,
		            saveToPhotoAlbum: false,
		            cameraDirection: Camera.DestinationType.FRONT
		        });

		    } catch (e) {

		    }
		};

		$scope.canShowCheckinVerify = function () {
			if($scope.saveCheckinResult && $scope.saveCheckinResult.success) {
				if ($scope.saveCheckinResult.success == true && $scope.needVarification) {
					$scope.bCheckinVerify = true;
				}
			}
			return $scope.bCheckinVerify;
		};

		// checkpoint modal
		var checkpointModal = null;
		$ionicModal.fromTemplateUrl('templates/edit-checkpoint.html', {
			scope: $scope,
			animation: 'slide-in-up',
			backdropClickToClose: false,
			focusFirstInput: true
		}).then(function (modal) {
			checkpointModal = modal;
		});

		$scope.$on('$destroy', function () {
			if (checkpointModal)
				checkpointModal.remove();
		});

		$scope.showCheckpointModal = function (checkpoint, sequence) {
			if (checkpointModal == null)
				return;
			var isNew = checkpoint == null;
			checkpoint = checkpoint || {};
			$scope.model.checkpoint = {
				sequence: sequence,
				isNew: isNew,
				name: checkpoint.name,
				percent: checkpoint.percent,
				estimatedDate: checkpoint.estimatedDate,
				original: checkpoint
			};
			checkpointModal.show();
		};

		$scope.saveCheckpoint = function (val) {
		    $scope.isSubmitted = true;
		    if(val){
			var checkpoint = $scope.model.checkpoint;
		    if (checkpoint.isNew) {
		        quest.checkpoints.push({
		            name: checkpoint.name,
		            percent: checkpoint.percent,
		            estimatedDate: checkpoint.estimatedDate,
		            deleted: false
		        });
		    } else {
		        var original = checkpoint.original;
		        original.name = checkpoint.name;
		        original.percent = checkpoint.percent;
		        original.estimatedDate = checkpoint.estimatedDate;
		    }

		    $scope.closeCheckpointModal();
		}
		};

		$scope.showPopup = function (msg) {
		    $scope.openPopup = $ionicPopup.show({
		        title: msg,
		        scope: $scope,
		        buttons: [
                    { text: 'Yes', type: 'button button-small button-calm', onTap: function () { $scope.deleteCheckpoint(); } },
                    { text: 'Cancel', type: 'button button-small button-calm', onTap: function () { } },
		        ]
		    });
		};

		$scope.deleteCheckpoint = function () {
			var checkpoint = $scope.model.checkpoint;
			if (!checkpoint.isNew)
				checkpoint.original.deleted = true;
			$scope.closeCheckpointModal();
		};

		$scope.closeCheckpointModal = function () {
			if (checkpointModal != null)
				checkpointModal.hide();
		}
		
		$scope.showGraph = function(chartType) {
			$scope.chartType = chartType;
			
			switch(chartType) {
			case 'map':
				$state.go('quest-map.chart-map', { questId: $stateParams.questId });
			break;
			
			case 'pulse':
				$state.go('quest-map.chart-pulse', { questId: $stateParams.questId });
			break;
			
			case 'timeline':
				$state.go('quest-map.chart-timeline', { questId: $stateParams.questId });
			break;
			}
		}

		NavService.setBackBtnTitle('Back');
}])
;
