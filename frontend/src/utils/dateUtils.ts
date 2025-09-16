/**
 * Date formatting utilities to ensure consistent date handling across the application
 */

import moment from "moment";

/**
 * Formats a date string to a localized date string for transaction dates.
 * Handles date strings that come from the API as date-only strings (YYYY-MM-DD).
 *
 * @param dateString - The date string from the API (e.g., "2028-01-01" or "2028-01-01T00:00:00")
 * @returns A formatted date string in the user's local timezone
 */
export function formatTransactionDate(dateString: string): string {
  // Check if the date is valid
  if (!dateString) {
    return "Invalid Date";
  }

  // For date-only strings like "2028-01-01", we want to avoid timezone issues
  // by parsing it as a local date instead of UTC
  let date: Date;

  if (dateString.includes("T")) {
    // Handle datetime strings like "2028-01-01T00:00:00"
    const parsedDate = new Date(dateString);
    if (isNaN(parsedDate.getTime())) {
      return "Invalid Date";
    }
    // Extract just the date components to avoid timezone shifts
    const year = parsedDate.getFullYear();
    const month = parsedDate.getMonth();
    const day = parsedDate.getDate();
    date = new Date(year, month, day);
  } else {
    // Handle date-only strings like "2028-01-01"
    const parts = dateString.split("-");
    if (parts.length !== 3) {
      return "Invalid Date";
    }
    const year = parseInt(parts[0], 10);
    const month = parseInt(parts[1], 10) - 1; // Month is 0-based in JavaScript
    const day = parseInt(parts[2], 10);

    if (isNaN(year) || isNaN(month) || isNaN(day)) {
      return "Invalid Date";
    }

    date = new Date(year, month, day);
  }

  if (isNaN(date.getTime())) {
    return "Invalid Date";
  }

  return date.toLocaleDateString("en-US", {
    year: "numeric",
    month: "short",
    day: "numeric",
  });
}

/**
 * Formats a date string to a more detailed localized format for transaction dates.
 * Similar to formatTransactionDate but with more detail (includes weekday).
 *
 * @param dateString - The date string from the API (e.g., "2028-01-01" or "2028-01-01T00:00:00")
 * @returns A formatted date string with weekday, month, day, year
 */
export function formatTransactionDateDetailed(dateString: string): string {
  if (!dateString) {
    return "Invalid Date";
  }

  let date: Date;

  if (dateString.includes("T")) {
    // Handle datetime strings like "2028-01-01T00:00:00"
    const parsedDate = new Date(dateString);
    if (isNaN(parsedDate.getTime())) {
      return "Invalid Date";
    }
    // Extract just the date components to avoid timezone shifts
    const year = parsedDate.getFullYear();
    const month = parsedDate.getMonth();
    const day = parsedDate.getDate();
    date = new Date(year, month, day);
  } else {
    // Handle date-only strings like "2028-01-01"
    const parts = dateString.split("-");
    if (parts.length !== 3) {
      return "Invalid Date";
    }
    const year = parseInt(parts[0], 10);
    const month = parseInt(parts[1], 10) - 1; // Month is 0-based in JavaScript
    const day = parseInt(parts[2], 10);

    if (isNaN(year) || isNaN(month) || isNaN(day)) {
      return "Invalid Date";
    }

    date = new Date(year, month, day);
  }

  if (isNaN(date.getTime())) {
    return "Invalid Date";
  }

  return date.toLocaleDateString("en-US", {
    weekday: "long",
    year: "numeric",
    month: "long",
    day: "numeric",
  });
}

/**
 * Formats a datetime string to a localized datetime string.
 * Used for timestamps like createdAt, updatedAt.
 * Converts UTC datetime to local timezone using moment.js.
 *
 * @param dateTimeString - The datetime string from the API (ISO format, typically UTC)
 * @returns A formatted datetime string in the user's local timezone
 */
export function formatDateTime(dateTimeString: string): string {
  if (!dateTimeString) {
    return "Invalid Date";
  }

  const momentDate = moment(dateTimeString);

  if (!momentDate.isValid()) {
    return "Invalid Date";
  }

  // Format in local timezone with clear indication
  return momentDate.format("MMM D, YYYY, h:mm A");
}

/**
 * Formats a datetime string to a localized datetime string with timezone information.
 * Used for timestamps like createdAt, updatedAt when explicit timezone display is needed.
 *
 * @param dateTimeString - The datetime string from the API (ISO format, typically UTC)
 * @returns A formatted datetime string in the user's local timezone with timezone abbreviation
 */
export function formatDateTimeWithTimezone(dateTimeString: string): string {
  if (!dateTimeString) {
    return "Invalid Date";
  }

  const momentDate = moment(dateTimeString);

  if (!momentDate.isValid()) {
    return "Invalid Date";
  }

  // Format in local timezone with timezone abbreviation
  return momentDate.format("MMM D, YYYY, h:mm A z");
}
