# Backend-3alegny
# API Endpoints Documentation

This document outlines the API endpoints for various user roles in the system.

---

## Authentication Endpoints

### POST
- **/signup**  
  Endpoint for user signup.

### GET
- **/login**  
  Endpoint for user login.

---

## Admin Endpoints

### POST
- **/create-business**  
  Create a business entity with three associated objects.

### GET
- **/patient-statistics**  
  Retrieve statistics about patients.  
- **/hospital-statistics**  
  Retrieve a list of hospitals with their statistics.  
- **/pharmacies-statistics**  
  Retrieve pharmacy-related statistics.  
- **/revenue-statistics**  
  Retrieve revenue-related statistics.  
- **/appointments-statistics**  
  Retrieve statistics on appointments.  
- **/order-statistics**  
  Retrieve order-related statistics.  
- **/patients-reviews**  
  Retrieve reviews from patients.

### UPDATE
- **/update-ehr-from-phr**  
  Update EHR data using PHR if needed.

### DELETE
- **/delete-business**  
  Delete an existing business.

---

## Hospital Endpoints

### POST
- **/post-departments**  
  Add new departments to the hospital.  
- **/post-doctors**  
  Add new doctors to the hospital.  
- **/post-ehr**  
  Create EHR for a patient for the first time.  
- **/add-offers**  
  Add new offers for hospital departments.

### GET
- **/current-patient-ehr**  
  Retrieve the current patient's EHR using their ID.  
- **/detailed-bill**  
  Retrieve a detailed bill for the current appointment (before confirmation).

### UPDATE
- **/update-ehr-and-phr**  
  Update both EHR and PHR after coding.  
- **/update-departments**  
  Update department details.  
- **/update-doctors**  
  Update doctor information.  
- **/update-offers**  
  Update offers for specific departments.

### DELETE
- **/delete-old-doctors**  
  Remove old doctors from the hospital.  
- **/delete-offers**  
  Remove offers from specific departments.

---

## Patient Endpoints

### POST
- **/create-phr**  
  Create a personal health record (PHR).  
- **/create-order**  
  Place an order.  
- **/create-appointment**  
  Book a new appointment.

### GET
- **/suggested-drugs**  
  Retrieve a list of suggested drugs for the patient.  
- **/order-history**  
  Retrieve a patient's order history.  
- **/appointment-history**  
  Retrieve a patient's appointment history.  
- **/phr**  
  Retrieve the patient's PHR.  
- **/current-location**  
  Retrieve the patient's current location.

### UPDATE
- **/update-phr**  
  Update the patient's PHR.  
- **/update-current-location**  
  Update the patient's current location.

### DELETE
- **/cancel-appointment**  
  Cancel an appointment.  
- **/cancel-order**  
  Cancel an order.

---

## Pharmacy Endpoints

### POST
- **/add-drugs**  
  Add new drugs to the pharmacy inventory.  
- **/add-offers**  
  Add new offers for the pharmacy.

### GET
- **/detailed-bill**  
  Retrieve a detailed bill for the current order (before confirmation).

### UPDATE
- **/new-drugs**  
  Update the list of drugs in the pharmacy.  
- **/new-offers**  
  Update the pharmacy offers.

### DELETE
- **/expired-drugs**  
  Remove expired drugs from inventory.  
- **/delete-offers**  
  Remove specific offers.

