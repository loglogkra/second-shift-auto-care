# Admin new service request notifications

The API can send a free-first admin email notification after a public service request has been saved successfully. Notifications are sent from the Azure Functions API only; no secrets are used by the Blazor WebAssembly client.

## Azure Static Web Apps environment variables

Required for email sending:

- `ResendApiKey`
- `NotificationFromEmail`
- `NotificationFromName`
- `AdminNotificationEmail`
- `PublicAppBaseUrl`
- `EnableAdminNewRequestNotifications=true`

Optional:

- `AdminNotificationSmsEmail` — treated as a secondary email recipient for email-to-SMS gateways if you choose to configure one later.

Example values:

- `PublicAppBaseUrl=https://auto.logankragt.com`
- `NotificationFromName=2nd Shift Auto Care`
- `AdminNotificationEmail=loganjkragt@gmail.com`

If `EnableAdminNewRequestNotifications` is false or missing, notifications are skipped. If `ResendApiKey` or `AdminNotificationEmail` is missing, notifications are skipped and logged. If Resend fails, the service request remains saved and the public submission still succeeds.

## API routes

- Public: `POST /api/service-requests`
- Public: `GET /api/health`
- Admin protected: `POST /api/manage/notifications/test`
- Admin protected: `/api/manage/*`

Do not add global auth redirects for API routes; Static Web Apps admin authorization should continue to protect `/api/manage/*` without breaking `/.auth/me`.

## Migration

Notification delivery attempts are logged in `NotificationLogs`.

Migration name: `AddAdminNewRequestNotifications`

Apply it with:

```bash
dotnet ef database update --project src/SecondShiftAutoCare.Api/SecondShiftAutoCare.Api.csproj --startup-project src/SecondShiftAutoCare.Api/SecondShiftAutoCare.Api.csproj --context ServiceRequestDbContext
```

## Deployment testing

1. Deploy without `ResendApiKey`; submit a public request and confirm it saves with only a logged notification skip.
2. Configure the required environment variables; submit a public request and confirm the admin email includes customer, vehicle, service, preferred availability, admin detail link, and customer status link when a public status token exists.
3. Temporarily use an invalid Resend key; submit a public request and confirm the request still succeeds and `NotificationLogs` records a failed delivery.
4. As an admin, `POST /api/manage/notifications/test` with `{ "toEmail": "you@example.com" }` or `{}` to test `AdminNotificationEmail`.
5. Confirm non-admin users receive `401` from `POST /api/manage/notifications/test`.
