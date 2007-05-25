var height = 0;
var width = 0;

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

function onLoad() 
{
	var px = 'px';
	var headerHeight = 130;
	var footerHeight = 25;
	var contactWidth = 250;
	var marginWidth = 300;
	
	// Set our global vars.
	getSize();

	// Resize the contents stuff.
	var contentHeight = (height - headerHeight - footerHeight);
	var contentWidth = (width - marginWidth);
	var halfContentWidth = ((width - 321) / 2);
	var archiveWidth = contentWidth - contactWidth - 15;
	var contactPosition = contentWidth - contactWidth;
	
	// Get our elements.
	var content = document.getElementById('content');
	var rightContent = document.getElementById('rightContent');
	var leftContent = document.getElementById('leftContent');
	var archives = document.getElementById('archives');
	var contact = document.getElementById('contact');
	
	// Increase the height.
	rightContent.style.height = contentHeight + px;
	leftContent.style.height = contentHeight + px;
	
	// Widen the content.
	content.style.width = contentWidth + px;
	rightContent.style.width = halfContentWidth + px;
	leftContent.style.width = halfContentWidth - 1 + px;
	
	// Show the archive footer and size it.
	archives.style.zIndex = 0;
	archives.style.width = archiveWidth + px;
	
	// Show the contact footer and size it and position it.
	contact.style.zIndex = 0;
	contact.style.width = contactWidth;
	contact.style.left = contactPosition + 145 + px;
}

function getSize() {
	// http://www.howtocreate.co.uk/tutorials/javascript/browserwindow
	if (typeof(window.innerWidth) == 'number') {
		//Non-IE
		height = window.innerHeight - 15;
		width = window.innerWidth - 15;
	} else if (document.documentElement && 
		(document.documentElement.clientWidth || document.documentElement.clientHeight)) {
		//IE 6+ in 'standards compliant mode'
		height = document.documentElement.clientHeight - 15;
		width = document.documentElement.clientWidth - 15;
	} else if (document.body && 
		(document.body.clientWidth || document.body.clientHeight)) {
		//IE 4 compatible
		height = document.body.clientHeight - 35;
		width = document.body.clientWidth - 35;
	}
}

