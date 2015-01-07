/* Services */
'use strict';

angular.module('maximuslife.services', ['ngResource'])
.value('ServerAddress', 'localhost:9000')
//.value('ServerAddress', '10.0.2.2:9000')
/* NavService handles 'menu items' and back button title. */
.factory('NavService', [ function() {
	var svcObj = {};
	
	svcObj.initVariables = function() {
		svcObj.menuItems = [
			{ text: 'Home', iconClass: 'icon ion-home', link: 'categories'},
			{ text: 'Settings', iconClass: 'icon ion-gear-b', link: 'settings'},
			{ text: 'Favourites', iconClass: 'icon ion-star', link: 'favourites'}
		];
		svcObj.backBtnTitle = '';
	};

	svcObj.getMenuItems = function() {
	  return svcObj.menuItems;
	};

	svcObj.setBackBtnTitle = function(backBtnTitle) {
	  svcObj.backBtnTitle = backBtnTitle;
	};

	svcObj.getBackBtnTitle = function() {
	  return svcObj.backBtnTitle;
	};
	
	/* Initial Execution Code */
	svcObj.initVariables();
	
	return svcObj;
}])
.factory('LoginService', ['$resource', 'ServerAddress', function ($resource, ServerAddress) {
    // Locals.
    var svcObj = {};

    svcObj.model = null;

    svcObj.setModel = function (model) {
        return svcObj.model = model;
    }

    svcObj.getModel = function () {
        return svcObj.model;
    }

    return svcObj;
}])
.factory('QuestService', ['$resource', 'ServerAddress', function ($resource, ServerAddress) {

    var svcObj = {};
    var rsrcCategories = $resource('http://' + ServerAddress + '/builder/categories/?format=json');
	var rsrcChildCategories = $resource('http://' + ServerAddress + '/builder/categories/:categoryId?format=json', {});
	var rsrcQuests = $resource('http://' + ServerAddress + '/builder/categories/:childCategoryId?format=json', {});
	var rsrcQuest = $resource('http://' + ServerAddress + '/quests/:questId?format=json', {});
	var rsrcSaveCheckin = $resource('http://' + ServerAddress + '/quests/:questId/checkins?format=json', {});
    
	/*
    ** Method to get categories.
    **
    ** @Returns - Returns the category list.
    */
    svcObj.getCategories = function () {
		return rsrcCategories.get();
    };

    /*
    ** Method to get category by Id
    ** @Param - {categoryId} - The category id.
    **
    ** @Returns - Returns a child categories which is matched to the passed in id.
    */
    svcObj.getCategory = function (categoryId) {
        return rsrcChildCategories.get({categoryId: categoryId});
    };

    /*
    ** Method to get quests
    ** @Param - {childCategoryId} - The child category id.
    **
    ** @Returns - Returns the list of quests which is matched to the passed in category id.
    */
    svcObj.getQuests = function (childCategoryId) {
        return rsrcQuests.get({childCategoryId: childCategoryId});
    };


    /*
    ** Method to get quest
    ** @Param - {questId} - The quest id.
    **
    ** @Returns - Returns a quest which is matched to the passed in quest id.
    */
    svcObj.getQuest = function (questId, success, error) {
    	return rsrcQuest.get({ questId: questId }, success, error);
    };

    svcObj.saveCheckin = function (checkin, success, error) {
        return rsrcSaveCheckin.save(checkin, success, error);
    };

    return svcObj;
}])
;