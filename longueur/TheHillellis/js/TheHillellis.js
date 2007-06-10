var px = 'px';
var height = 0;
var width = 0;
var contentHeight = 0;
var contentWidth = 0;
var numberOfArchives = 2;
var calculatedFooterHeight = 0;
var calculatedFooterBottom = 0;
var archiveWidth = 0;

// These are dependant on the browser being used.
var heightOfArchiveItems = 36;
var yPositionOfContent = 0;

function addListener(element, event, listener, bubble) {
	if(element.addEventListener) {
		if(typeof(bubble) == 'undefined') {
			bubble = false;
		} 
		
		element.addEventListener(event, listener, bubble);
	} else if(this.attachEvent) {
		element.attachEvent('on' + event, listener);
	}
}

function changeBackgroundColor(elementId, color) {
	if (document.getElementById(elementId) != null) {
		var element = document.getElementById(elementId);
		element.style.backgroundColor = color;
	}
}

function onLoad() {
	var headerHeight;
	var footerHeight = 25;
	var marginWidth = 299;
	
	// Set our global vars.
	getSize();
	
	// Set our header height base don the width.
	if (width < 850) {
		document.getElementById('logo').innerHTML = '<img src=\'images/logo_t.png\' alt=\'logo\' title=\'Wherein we make fun of lots of random stuff.\' />';
		headerHeight = 65;
	} else {
		document.getElementById('logo').innerHTML = '<img src=\'images/logo.png\' alt=\'logo\' title=\'Wherein we make fun of lots of random stuff.\' />';
		headerHeight = 90;
	}
	
	// Resize the content.
	var content = document.getElementById('content');
	contentHeight = (height - headerHeight - footerHeight) + 3;
	contentWidth = (width - marginWidth);
	content.style.width = contentWidth + px;
	
	// Calculate the footer's correct height.
	calculatedFooterHeight = footerHeight + (numberOfArchives * heightOfArchiveItems);
	calculatedFooterBottom = '-' + (calculatedFooterHeight - 10);
	
	// Define how wide the contact box is (need for to calculate the archive info).
	var contact = document.getElementById('contact');
	var rightArchivesLeftPosition;
	
	// Show the contact footer and size it and position it based on the window width.
	if (width < 915) {
		archiveWidth = (contentWidth - 15) / 2;
		rightArchivesLeftPosition = yPositionOfContent + 10 + archiveWidth;
		
		resizeContactElement(contentWidth - 5, yPositionOfContent, calculatedFooterBottom - calculatedFooterHeight - 10);
	} else {
		var contactWidth = 250;
		archiveWidth = (contentWidth - contactWidth - 15) / 2;
		
		var contactPosition = yPositionOfContent + archiveWidth + 10;
		rightArchivesLeftPosition = contactPosition + contactWidth;
		
		resizeContactElement(contactWidth - 10, contactPosition, calculatedFooterBottom);		
	}
	
	// Show the archive footer and size it.
	resizeFooterElement('leftArchives', yPositionOfContent);
	resizeFooterElement('rightArchives', rightArchivesLeftPosition);
	
	var url = location.href;
	var filename = url.substring(url.lastIndexOf('/') + 1, url.length).toLowerCase();
	
	if (filename.lastIndexOf('?') > -1) {
		filename = filename.substring(0, filename.lastIndexOf('?'));
	}
	
	// This seems pretty lame, but I need a way to resize the content based on different url's.
	if (filename == '' ||
		filename == 'default.aspx') {
		resizeDefaultContent();
	} else if (filename == 'permalink.aspx') {
		resizeOneContentBox('permalinkContent');
	} else if (filename == 'archive.aspx') {
		resizeOneContentBox('archiveContent');
	}
}

function resizeFooterElement(elementId, leftPosition) {
	var element = document.getElementById(elementId);
	element.style.zIndex = 0;
	element.style.width = archiveWidth + px;
	element.style.height = calculatedFooterHeight + px;
	element.style.bottom = calculatedFooterBottom + px;
	element.style.left = leftPosition + px;	
}

function resizeContactElement(width, leftPosition, bottom) {
	var contact = document.getElementById('contact');
	contact.style.zIndex = 0;
	contact.style.width = width + px;
	contact.style.height = calculatedFooterHeight + px;
	contact.style.bottom = bottom + px;
	contact.style.left = leftPosition + px;
}

function resizeDefaultContent() {
	var halfContentWidth = ((contentWidth - 16) / 2);
	
	// Get our elements.
	var rightContent = document.getElementById('rightContent');
	var leftContent = document.getElementById('leftContent');
	
	// Increase the height.
	rightContent.style.height = contentHeight + px;
	leftContent.style.height = contentHeight + px;
	
	// Widen the content.
	rightContent.style.width = halfContentWidth + px;
	leftContent.style.width = halfContentWidth - 1 + px;
}

function resizeOneContentBox(contentId) {
	// Get our elements.
	var content = document.getElementById(contentId);
	
	// Increase the height.
	content.style.height = contentHeight + px;
	
	// Widen the content.
	content.style.width = (contentWidth - 6) + px;
}

function getSize() {
	// http://www.howtocreate.co.uk/tutorials/javascript/browserwindow
	if (typeof(window.innerWidth) == 'number') {
		//Non-IE
		height = window.innerHeight - 15;
		width = window.innerWidth - 15;
		yPositionOfContent = 148;
	} else if (document.documentElement && 
		(document.documentElement.clientWidth || document.documentElement.clientHeight)) {
		//IE 6+ in 'standards compliant mode'
		height = document.documentElement.clientHeight - 15;
		width = document.documentElement.clientWidth - 15;
		yPositionOfContent = 156;
		heightOfArchiveItems = 42;
	} else if (document.body && 
		(document.body.clientWidth || document.body.clientHeight)) {
		//IE 4 compatible
		height = document.body.clientHeight - 35;
		width = document.body.clientWidth - 35;
	}
}

