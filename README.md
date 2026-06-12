# PHED Chhattisgarh eMB Management System

## Overview

The PHED Chhattisgarh eMB Management System is a web-based application developed to streamline the management of engineering measurement books (eMB) and related processes for the Public Health Engineering Department (PHED) in Chhattisgarh, India. Built using ASP.NET Web Forms, this system enhances efficiency and accuracy in tracking project progress, managing agreements, generating bills, and facilitating approvals for engineering projects.

## Project Purpose

The application aims to:

- Digitize the recording and tracking of engineering measurements.
- Automate progress reporting and bill generation.
- Provide a structured approval workflow for eMB books.
- Offer a centralized dashboard for monitoring key metrics.

## Project Structure

The system comprises several ASPX pages, each designed for specific functionalities:

- **Login.aspx**: User authentication entry point.
- **frmHome.aspx**: Dashboard displaying key statistics (e.g., pending approvals, approved/rejected eMBs).
- **AgreementList.aspx**: Search and manage agreements by eMB book number and work code.
- **Components.aspx**: Lists components of a work code and allows physical progress entry.
- **ComponentList.aspx**: Displays sub-components for measurement entry.
- **eMBEntry.aspx**: Interface for entering detailed measurements.
- **ProgressReport.aspx**: Generates detailed progress reports for eMB books and work codes.
- **Report01.aspx**: Provides division-wise eMB issuance and initiation reports.
- **GenerateBill.aspx**: Facilitates bill creation and management.
- **Approval.aspx**: Manages the multi-level approval process for eMB books.

## Key Features

- **User Authentication**: Secure login with captcha verification (`Login.aspx`).
- **Dashboard Overview**: Displays real-time statistics on eMB status and engineer activities (`frmHome.aspx`).
- **Agreement Management**: Search agreements via direct input or dropdowns (`AgreementList.aspx`).
- **Component Tracking**: Manage components and sub-components with progress updates (`Components.aspx`, `ComponentList.aspx`).
- **Measurement Entry**: Detailed entry with formula-based calculations (`eMBEntry.aspx`).
- **Progress Reporting**: Visualize project progress with percentages and quantities (`ProgressReport.aspx`).
- **Division Reports**: Summarize eMB issuance and initiation by division (`Report01.aspx`).
- **Bill Generation**: Create detailed bills with component breakdowns (`GenerateBill.aspx`).
- **Approval Workflow**: Multi-tier approval process (Sub-Engineer, Assistant Engineer, Executive Engineer) (`Approval.aspx`).

## Workflow

1. **Login**:

   - Users authenticate via `Login.aspx` using credentials and captcha.

2. **Dashboard**:

   - Post-login, users land on `frmHome.aspx`, viewing metrics like pending approvals and total eMBs.

3. **Agreement Search**:

   - In `AgreementList.aspx`, users search for agreements using:
     - **Direct Search**: Enter eMB book number and optional work code.
     - **Dropdown Search**: Select from predefined lists.
   - Results display work details, leading to component selection.

4. **Component Management**:

   - `Components.aspx` lists components for a selected work code, allowing progress entry or sub-component viewing.
   - `ComponentList.aspx` shows sub-components, linking to measurement entry.

5. **Measurement Entry**:

   - `eMBEntry.aspx` enables users to input measurements using predefined formulas, tracking remaining quantities.

6. **Progress Reporting**:

   - `ProgressReport.aspx` generates reports showing completed quantities, percentages, and remaining work.

7. **Division-Wise Reporting**:

   - `Report01.aspx` provides an overview of eMB issuance and initiation across divisions.

8. **Bill Generation**:

   - `GenerateBill.aspx` allows users to:
     - Search work codes.
     - View bill history.
     - Create new bills with detailed breakdowns (components, GST, totals).

9. **Approval Process**:

   - `Approval.aspx` facilitates a three-tier approval:
     - **Sub-Engineer (SE)**: Initial submission.
     - **Assistant Engineer (AE)**: First-level review.
     - **Executive Engineer (EE)**: Final approval.
   - Status updates are reflected in real-time.

## Technical Details

- **Framework**: ASP.NET Web Forms
- **Language**: C#
- **Database**: SQL Server (assumed, based on typical ASP.NET setups)
- **Frontend**: HTML, CSS, JavaScript, Bootstrap
- **AJAX**: Enhances user experience with asynchronous updates
- **Security**: Role-based access control and captcha-based login

## Installation and Setup

1. **Clone the Repository**:

   ```bash
   git clone <repository-url>
   ```
2. **Database Setup**:
   - Configure SQL Server with the provided schema.
   - Import seed data if available.
3. **Configuration**:
   - Update `web.config` with database connection strings and settings.
4. **Build and Run**:
   - Open in Visual Studio, build the solution, and run via IIS or the development server.

## Usage

- **Login**: Enter credentials and captcha in `Login.aspx`.
- **Dashboard**: Monitor metrics on `frmHome.aspx`.
- **Search Agreements**: Use `AgreementList.aspx` to find work codes.
- **Manage Components**: Navigate through `Components.aspx` and `ComponentList.aspx`.
- **Enter Measurements**: Input data in `eMBEntry.aspx`.
- **Generate Reports**: View progress in `ProgressReport.aspx` or division reports in `Report01.aspx`.
- **Create Bills**: Use `GenerateBill.aspx` for billing.
- **Approve eMBs**: Review and approve in `Approval.aspx`.

## License

This project is licensed under the MIT License.