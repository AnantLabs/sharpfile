var px = 'px';
var height = 0;
var width = 0;
var contentHeight = 0;
var contentWidth = 0;
var numberOfArchives = 1;

// These are dependant on the browser being used.
var heightOfArchiveItems = 18;
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
	var headerHeight = 130;
	var footerHeight = 25;
	var marginWidth = 300;
	
	// Set our global vars.
	getSize();
	
	// Resize the content.
	var content = document.getElementById('content');
	contentHeight = (height - headerHeight - footerHeight);
	contentWidth = (width - marginWidth);
	content.style.width = contentWidth + px;
	
	// Calculate the footer's correct height.
	var calculatedFooterHeight = footerHeight + (numberOfArchives * heightOfArchiveItems);
	var calculatedFooterBottom = '-' + (calculatedFooterHeight - 10);
	
	// Define how wide the contact box is (need for to calculate the archive info).
	var contactWidth = 250;
	
	// Show the archive footer and size it.
	var archives = document.getElementById('archives');
	var archiveWidth = contentWidth - contactWidth - 15;
	archives.style.zIndex = 0;
	archives.style.width = archiveWidth + px;
	archives.style.left = yPositionOfContent + px;
	archives.style.height = calculatedFooterHeight + px;
	archives.style.bottom = calculatedFooterBottom + px;
	
	// Show the contact footer and size it and position it.
	var contact = document.getElementById('contact');
	var contactPosition = yPositionOfContent + archiveWidth;
	contact.style.zIndex = 0;
	contact.style.width = contactWidth;
	contact.style.left = contactPosition + 10 + px;
	contact.style.height = calculatedFooterHeight + px;
	contact.style.bottom = calculatedFooterBottom + px;
	
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
		resizePermalinkContent();
	}
}

function resizeDefaultContent() {
	var halfContentWidth = ((contentWidth - 21) / 2);
	
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

function resizePermalinkContent() {
	// Get our elements.
	var permalinkContent = document.getElementById('permalinkContent');
	
	// Increase the height.
	permalinkContent.style.height = contentHeight + px;
	
	// Widen the content.
	permalinkContent.style.width = (contentWidth - 6) + px;
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
		yPositionOfContent = 155;
		heightOfArchiveItems = 21;
	} else if (document.body && 
		(document.body.clientWidth || document.body.clientHeight)) {
		//IE 4 compatible
		height = document.body.clientHeight - 35;
		width = document.body.clientWidth - 35;
	}
}

