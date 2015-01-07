define(function() {
	var CartItem = function(item) {
		var self = this;

		item = item || {};

		self.product = ko.observable(item.product);
		self.quantity = ko.observable(item.quantity == null ? 1 : item.quantity);
		self.amount = ko.computed(function() {
			if(!self.product())
				return 0;
			var quantity = parseInt("0" + self.quantity());
			return self.product().price * quantity;
		});
	};

	var Address = function(addr) {
		var self = this;

		addr = addr || {};

		self.street = ko.observable(addr.street || '');
		self.city = ko.observable(addr.city || '');
		self.state = ko.observable(addr.state || '');
		self.zip = ko.observable(addr.zip || '');
	};

	Address.prototype.copyTo = function(other) {
		other.street(this.street());
		other.city(this.city());
		other.state(this.state());
		other.zip(this.zip());
	};

	var Cart = function(options, cart) {
		var self = this;

		cart = cart || {};

		self.products = options.products;
		self.shippingMethods = options.shippingMethods;

		var items = ko.utils.arrayMap(cart.items || [], function (item) { return new CartItem(item); });

		if (items.length == 0)
			items = [new CartItem()];

		self.items = ko.observableArray(items);
		self.billingAddress = new Address(cart.billingAddress);
		self.shippingAddress = new Address(cart.shippingAddress);
		self.shippingSameAsBilling = ko.observable();

		self.shippingMethod = ko.observable(cart.shippingMethod || options.shippingMethods[0]);

		self.subtotal = ko.computed(function() {
			var amount = 0;
			ko.utils.arrayForEach(self.items(), function(item) { amount += item.amount(); });
			return amount;
		});

		self.shipping = ko.computed(function() {
			return self.subtotal() > 0 ? self.shippingMethod().cost : 0;
		});

		self.tax = ko.computed(function() {
			return .0825 * self.subtotal();
		});

		self.total =  ko.computed(function() {
			return self.subtotal() + self.shipping() + self.tax();
		});

		self.addItem = function() { self.items.push(new CartItem()); }

		self.removeItem = function(item) { self.items.remove(item); }

		self.shippingSameAsBilling.subscribe(function(same) {
			if(same)
				self.billingAddress.copyTo(self.shippingAddress);
		});

		ko.utils.arrayForEach([ 'street', 'city', 'state', 'zip' ], function(prop) {
			self.billingAddress[prop].subscribe(function(value) {
				if(self.shippingSameAsBilling()) self.shippingAddress[prop](value);
			});
		});

		self.formatCurrency = function(value) {
			return "$" + value.toFixed(2);
		};
	}

	return Cart;
});