interface Props {
  message?: string
  size?: 'sm' | 'md' | 'lg'
}

export function LoadingSpinner({ message = 'Loading...', size = 'md' }: Props) {
  const sizes = { sm: 'h-4 w-4', md: 'h-8 w-8', lg: 'h-12 w-12' }

  return (
    <div className="flex flex-col items-center justify-center gap-3 py-12 text-gray-500">
      <div
        className={`${sizes[size]} animate-spin rounded-full border-2 border-gray-200 border-t-blue-600`}
      />
      <span className="text-sm">{message}</span>
    </div>
  )
}
