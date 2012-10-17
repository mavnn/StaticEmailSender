# Static Email Sender

## What does it do?

Compile and run the console app. You will need to set up your own smtp server address in app.config.

If you hit an address like http://servername:64537/emailName/my.email.address@example.com it will then send you an email at "my.email.address@example.com". If it can find a file "emailName.html" in the same directory it will send that to you as an html email, other wise it will send an email with a plain text body of "emailName".

## Why?

I thought it was going to useful, but then found out one of my colleges had just built the functionality straight into the demo website in question.

I preferred my version, though (mostly because it's mine, I suspect) and was also pleased at how short it ended up.